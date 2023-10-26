using AutoMapper;
using BestReads.Core;
using BestReads.Core.Constants;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads.Tests.Fakers;
using Bogus;
using Microsoft.Extensions.Logging;

namespace BestReads.Tests.Features.BookRecommendations.Services;

public class BookRecommendationServiceTests
{
    private readonly Mock<IBookEmbeddingService> _mockBookEmbeddingService;
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<ILogger<BookRecommendationService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BookRecommendationService _service;

    public BookRecommendationServiceTests()
    {
        _mockBookEmbeddingService = new Mock<IBookEmbeddingService>();
        _mockBookRepository = new Mock<IBookRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BookRecommendationService>>();

        _service = new BookRecommendationService(_mockBookRepository.Object, _mockBookEmbeddingService.Object,
            _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateRecommendations_BookInRepo_ShouldReturnSuccessResponse()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var book = BookFakers.BookModelFaker().Generate();
        _mockBookRepository.Setup(repo => repo.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId))
            .ReturnsAsync(book);

        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateRecommendations_BookNotInRepo_ShouldFetchEmbeddingsAndReturnSuccessResponse()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var book = BookFakers.BookModelFaker().Generate();
        var embeddingRequest = new EmbeddingRequest();
        var embeddings = new List<float> { 0.1f, 0.2f };

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

        //Todo: update these tests - obtain recommendations from Vector Search (bestreads-extensions #5)
        // Act
        var result = await _service.GenerateRecommendations(bookRecommendationsDto);

        // Assert
        result.Success.Should().BeTrue();
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
}