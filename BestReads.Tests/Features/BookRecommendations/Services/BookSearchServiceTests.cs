using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Services.BookSearchService;
using BestReads.Infrastructure.AzureSearchClient;
using BestReads.Tests.Fakers;

namespace BestReads.Tests.Features.BookRecommendations.Services;

public class BookSearchServiceTests
{
    private readonly Mock<IAzureSearchClient> _azureSearchClientMock;
    private readonly BookSearchService _bookSearchService;

    public BookSearchServiceTests()
    {
        _azureSearchClientMock = new Mock<IAzureSearchClient>();
        _bookSearchService = new BookSearchService(_azureSearchClientMock.Object);
    }

    [Fact]
    public async Task GetNearestNeighbors_WhenCalled_ReturnsExpectedRecommendations()
    {
        // Arrange
        var book = BookFakers.BookModelFaker().Generate();
        var expectedRecommendations = BookFakers.BookRecommendationDtoFaker().Generate(5);

        _azureSearchClientMock
            .Setup(x => x.SingleVectorSearch<BookRecommendationDto>(
                It.IsAny<float[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<string>>()))
            .ReturnsAsync(expectedRecommendations);

        // Act
        var result = await _bookSearchService.GetNearestNeighbors(book);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedRecommendations);

        var expectedFilter = $"GoogleBooksId ne '{book.GoogleBooksId}'";
        var expectedSelectOptions = new List<string>
        {
            "doc_id, GoogleBooksId, Title, Authors, Categories, Description, Publisher, PublishedDate, Thumbnail, IndustryIdentifiers"
        };
        _azureSearchClientMock.Verify(x => x.SingleVectorSearch<BookRecommendationDto>(
            book.Embeddings,
            "Embeddings",
            expectedFilter,
            expectedSelectOptions), Times.Once);
    }
}