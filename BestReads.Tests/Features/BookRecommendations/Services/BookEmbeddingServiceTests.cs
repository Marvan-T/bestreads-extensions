using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;
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
        result.IsSuccess.Should().BeTrue();
        var expectedText = $"Title: {fakeBookDto.Title};" +
                           $"Authors: {string.Join(", ", fakeBookDto.Authors)};" +
                           $"Categories: {string.Join(", ", fakeBookDto.Categories)};" +
                           $"Description: {fakeBookDto.Description};";
        result.Data.Text.Should().Be(expectedText);
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ConstructEmbeddingRequest_Should_Return_Failure_When_Title_Is_Invalid(string title)
    {
        // Arrange
        var fakeBookDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        fakeBookDto.Title = title;

        // Act
        var result = _bookEmbeddingService.ConstructEmbeddingRequest(fakeBookDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GenerateRecommendationErrors.TitleRequired);
        _openAiClientMock.Verify(x => x.GetEmbeddingsAsync(It.IsAny<EmbeddingRequest>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(InvalidListData))]
    public void ConstructEmbeddingRequest_Should_Return_Failure_When_Authors_Are_Invalid(List<string> authors)
    {
        // Arrange
        var fakeBookDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        fakeBookDto.Authors = authors;

        // Act
        var result = _bookEmbeddingService.ConstructEmbeddingRequest(fakeBookDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GenerateRecommendationErrors.AuthorsRequired);
        _openAiClientMock.Verify(x => x.GetEmbeddingsAsync(It.IsAny<EmbeddingRequest>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(InvalidListData))]
    public void ConstructEmbeddingRequest_Should_Return_Failure_When_Categories_Are_Invalid(List<string> categories)
    {
        // Arrange
        var fakeBookDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        fakeBookDto.Categories = categories;

        // Act
        var result = _bookEmbeddingService.ConstructEmbeddingRequest(fakeBookDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GenerateRecommendationErrors.CategoriesRequired);
        _openAiClientMock.Verify(x => x.GetEmbeddingsAsync(It.IsAny<EmbeddingRequest>()), Times.Never);
    }

    public static IEnumerable<object[]> InvalidListData()
    {
        yield return new object[] { new List<string>() }; // Empty list
        yield return new object[] { null }; // Null list
        yield return new object[] { new List<string> { "", " " } }; // List with empty or whitespace strings
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ConstructEmbeddingRequest_Should_Return_Failure_When_Description_Is_Invalid(string description)
    {
        // Arrange
        var fakeBookDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        fakeBookDto.Description = description;

        // Act
        var result = _bookEmbeddingService.ConstructEmbeddingRequest(fakeBookDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GenerateRecommendationErrors.DescriptionRequired);
        _openAiClientMock.Verify(x => x.GetEmbeddingsAsync(It.IsAny<EmbeddingRequest>()), Times.Never);
    }

}