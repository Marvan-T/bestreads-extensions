using BestReads_Recommendations.Core;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Features.BookRecommendations.Services.BookEmbeddingService;

public class BookEmbeddingService : IBookEmbeddingService
{
    private readonly IOpenAICleint _openAiCleint;

    public BookEmbeddingService(IOpenAICleint openAiCleint)
    {
        _openAiCleint = openAiCleint;
    }

    public EmbeddingRequest ConstructEmbeddingRequest(GetBookRecommendationsDto bookRecommendationsDto)
    {
        var embeddingText = $"Title: {bookRecommendationsDto.Title};" +
                            $"Authors: {string.Join(", ", bookRecommendationsDto.Authors)};" +
                            $"Categories: {string.Join(", ", bookRecommendationsDto.Categories)};" +
                            $"Description: {bookRecommendationsDto.Description};";

        return new EmbeddingRequest
        {
            Text = embeddingText
        };
    }

    public async Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request)
    {
        var response = await _openAiCleint.GetEmbeddingsAsync(request);
        return response.Value.Data[0].Embedding;
    }
}