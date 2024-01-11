using AutoMapper;
using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads.Features.BookRecommendations.Services.BookSearchService;
using BestReads.Tests.Fakers;
using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BestReads.Tests.Features.BookRecommendations.Services;

public class BookRecommendationServiceTests
{
    private readonly Mock<IBookEmbeddingService> _mockBookEmbeddingService;
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IBookSearchService> _mockBookSearchService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<BookRecommendationService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BookRecommendationService _service;


    public BookRecommendationServiceTests()
    {
        _mockBookEmbeddingService = new Mock<IBookEmbeddingService>();
        _mockBookRepository = new Mock<IBookRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BookRecommendationService>>();
        _mockBookSearchService = new Mock<IBookSearchService>();
        _mockConfiguration = new Mock<IConfiguration>();

        _service = new BookRecommendationService(_mockBookRepository.Object, _mockBookEmbeddingService.Object,
            _mockMapper.Object, _mockLogger.Object, _mockBookSearchService.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task GenerateRecommendations_BookInRepo_ShouldNotFetchEmbeddingsAndReturnRecommendations()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var book = BookFakers.BookModelFaker().Generate();
        var recommendations = BookFakers.BookRecommendationDtoFaker().Generate(5);
        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync(book);
        _mockBookSearchService.Setup(searchService => searchService.GetNearestNeighbors(book))
            .ReturnsAsync(Result<List<BookRecommendationDto>>.Success(recommendations));

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(recommendations);
        result.Error.Should().BeEquivalentTo(Error.None);
        _mockBookEmbeddingService.Verify(
            embeddingService => embeddingService.GetEmbeddingsFromOpenAI(It.IsAny<EmbeddingRequest>()), Times.Never());
        _mockBookEmbeddingService.Verify(
            embeddingService => embeddingService.ConstructEmbeddingRequest(It.IsAny<GetBookRecommendationsDto>()),
            Times.Never());
    }


    [Fact]
    public async Task GenerateRecommendations_BookNotInRepo_ShouldFetchEmbeddingsAndReturnSuccessResponse()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var book = BookFakers.BookModelFaker().Generate();
        var embeddingRequest = Result<EmbeddingRequest>.Success(new EmbeddingRequest());
        var embeddings = new List<float> { 0.1f, 0.2f };
        var recommendations =
            Result<List<BookRecommendationDto>>.Success(BookFakers.BookRecommendationDtoFaker().Generate(5));

        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync((Book)null);
        _mockBookEmbeddingService.Setup(service => service.ConstructEmbeddingRequest(bookRecommendationsDto))
            .Returns(embeddingRequest);
        _mockBookEmbeddingService.Setup(service => service.GetEmbeddingsFromOpenAI(embeddingRequest.Data))
            .ReturnsAsync(embeddings);
        _mockMapper.Setup(mapper => mapper.Map<Book>(bookRecommendationsDto))
            .Returns(book);
        _mockBookRepository.Setup(repo => repo.StoreBookAsync(book))
            .Returns(Task.CompletedTask);
        _mockBookSearchService.Setup(service => service.GetNearestNeighbors(book))
            .ReturnsAsync(recommendations);

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(recommendations.Data);
        result.Error.Should().BeEquivalentTo(Error.None);
    }

    [Fact]
    public async Task GenerateRecommendations_Exception_ShouldReturnErrorResponse()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var exceptionMessage = new Faker().Lorem.Sentence();
        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .Throws(new Exception(exceptionMessage));

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(new Error("UnexpectedError", exceptionMessage));
        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, type) =>
                    state.ToString() ==
                    $"Error while generating recommendations for {bookRecommendationsDto.GoogleBooksId}"),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once()
        );
    }

    [Fact]
    public async Task GenerateRecommendations_ReturnsOnlyFiveUniqueRecommendations()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var book = BookFakers.BookModelFaker().Generate();
        var uniqueRecommendations = BookFakers.BookRecommendationDtoFaker().Generate(8);
        var duplicateRecommendations = new List<BookRecommendationDto>
        {
            uniqueRecommendations[0],
            uniqueRecommendations[1]
        };
        var recommendations = uniqueRecommendations.Concat(duplicateRecommendations).ToList();

        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync(book);
        _mockBookSearchService.Setup(service => service.GetNearestNeighbors(book))
            .ReturnsAsync(Result<List<BookRecommendationDto>>.Success(recommendations));

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Count.Should().Be(5);
        result.Data.Should().OnlyHaveUniqueItems(r => r.Title);
    }

    [Fact]
    public async Task GenerateRecommendations_MissingGoogleBooksId_ShouldReturnFailure()
    {
        // Arrange
        var bookRecommendationsDto = new GetBookRecommendationsDto(); // Empty or null GoogleBooksId

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GenerateRecommendationErrors.GoogleBooksIdNotFound);
        _mockBookEmbeddingService.Verify(
            embeddingService => embeddingService.GetEmbeddingsFromOpenAI(It.IsAny<EmbeddingRequest>()), Times.Never());
        _mockBookEmbeddingService.Verify(
            embeddingService => embeddingService.ConstructEmbeddingRequest(It.IsAny<GetBookRecommendationsDto>()),
            Times.Never());
        _mockBookSearchService.Verify(
            searchService => searchService.GetNearestNeighbors(It.IsAny<Book>()), Times.Never());
    }

    [Fact]
    public async Task GenerateRecommendations_FailedEmbeddingGeneration_ShouldReturnFailure()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync((Book)null);
        _mockBookEmbeddingService.Setup(service => service.ConstructEmbeddingRequest(bookRecommendationsDto))
            .Returns(Result<EmbeddingRequest>.Failure(GenerateRecommendationErrors.TitleRequired));

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GenerateRecommendationErrors.TitleRequired);
        _mockBookEmbeddingService.Verify(
            embeddingService => embeddingService.GetEmbeddingsFromOpenAI(It.IsAny<EmbeddingRequest>()), Times.Never());
        _mockBookSearchService.Verify(
            searchService => searchService.GetNearestNeighbors(It.IsAny<Book>()), Times.Never());
    }

    [Fact]
    public async Task GetRecommendationsAsync_WhenThumbnailIsEmpty_ShouldSetDefaultThumbnail()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var book = BookFakers.BookModelFaker().Generate();
        var recommendations = BookFakers.BookRecommendationDtoFaker().Generate(8).ToList();
        recommendations.ForEach(r => r.Thumbnail = string.Empty); // Set all thumbnails to empty
        var defaultThumbnailUrl = "https://default-thumbnail-url.com";
        
        _mockConfiguration.Setup(config => config["DEFAULT_THUMBNAIL_URL"]).Returns(defaultThumbnailUrl);
        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync(book);
        _mockBookSearchService.Setup(service => service.GetNearestNeighbors(book))
            .ReturnsAsync(Result<List<BookRecommendationDto>>.Success(recommendations));

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().OnlyContain(r => r.Thumbnail == defaultThumbnailUrl);
    }
    
}