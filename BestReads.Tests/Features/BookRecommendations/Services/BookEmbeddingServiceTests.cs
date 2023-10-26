using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Tests.Fakers;

namespace BestReads.Tests.Features.BookRecommendations.Services;

public class BookEmbeddingServiceTests
{
    private readonly BookEmbeddingService _bookEmbeddingService;
    private readonly Mock<IOpenAICleint> _openAiClientMock;

    public BookEmbeddingServiceTests()
    {
        _openAiClientMock = new Mock<IOpenAICleint>();
        _bookEmbeddingService = new BookEmbeddingService(_openAiClientMock.Object);
    }

    [Fact]
    public void ConstructEmbeddingRequest_Should_Construct_EmbeddingRequest_Correctly()
    {
        // Arrange
        var fakeBookDto = BookFakers.GetBookRecommendationDtoFaker().Generate();

        // Act
        var result = _bookEmbeddingService.ConstructEmbeddingRequest(fakeBookDto);

        // Assert
        var expectedText = $"Title: {fakeBookDto.Title};" +
                           $"Authors: {string.Join(", ", fakeBookDto.Authors)};" +
                           $"Categories: {string.Join(", ", fakeBookDto.Categories)};" +
                           $"Description: {fakeBookDto.Description};";
        result.Text.Should().Be(expectedText);
    }

    [Fact]
    public async Task GetEmbeddingsFromOpenAI_Should_Return_Embeddings_When_API_Returns_Data()
    {
        // Arrange
        var fakeRequest = new EmbeddingRequest { Text = "SomeText" };
        var expectedEmbeddings = new List<float> { 0.5f, 1.5f };
        _openAiClientMock.Setup(x => x.GetEmbeddingsAsync(fakeRequest)).ReturnsAsync(expectedEmbeddings);

        // Act
        var result = await _bookEmbeddingService.GetEmbeddingsFromOpenAI(fakeRequest);

        // Assert
        result.Should().Equal(expectedEmbeddings);
        _openAiClientMock.Verify(x => x.GetEmbeddingsAsync(fakeRequest), Times.Once);
    }
}