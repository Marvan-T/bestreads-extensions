using AutoMapper;
using BestReads.Core;
using BestReads.Core.Constants;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads.Features.BookRecommendations.Services.BookSearchService;
using BestReads.Tests.Fakers;
using Bogus;
using Microsoft.Extensions.Logging;

namespace BestReads.Tests.Features.BookRecommendations.Services;

public class BookRecommendationServiceTests
{
    private readonly Mock<IBookEmbeddingService> _mockBookEmbeddingService;
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IBookSearchService> _mockBookSearchService;
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

        _service = new BookRecommendationService(_mockBookRepository.Object, _mockBookEmbeddingService.Object,
            _mockMapper.Object, _mockLogger.Object, _mockBookSearchService.Object);
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
            .ReturnsAsync(recommendations);

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(recommendations);
        result.Errors.Should().BeEmpty();
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
        var embeddingRequest = new EmbeddingRequest();
        var embeddings = new List<float> { 0.1f, 0.2f };
        var recommendations = BookFakers.BookRecommendationDtoFaker().Generate(5);

        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync((Book)null);
        _mockBookEmbeddingService.Setup(service => service.ConstructEmbeddingRequest(bookRecommendationsDto))
            .Returns(embeddingRequest);
        _mockBookEmbeddingService.Setup(service => service.GetEmbeddingsFromOpenAI(embeddingRequest))
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
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(recommendations);
        result.Errors.Should().BeEmpty();
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
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(ErrorCodes.Feature_BookRecommendations.GenerateRecommendationsError,
            exceptionMessage);
        //  Moq does not directly support verifying extension methods. This verifys  against the base Log method that the extension method (LogError) calls
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
            .ReturnsAsync(recommendations);

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.Data.Count.Should().Be(5);
        result.Data.Should().OnlyHaveUniqueItems(r => r.Title);
        result.Success.Should().Be(true);
    }
}