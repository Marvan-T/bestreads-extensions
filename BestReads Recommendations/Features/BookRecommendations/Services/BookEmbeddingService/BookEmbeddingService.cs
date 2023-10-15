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

    public EmbeddingRequest ConstructEmbeddingRequest(Book book)
    {
        var embeddingText = $"Title: {book.Title};" +
                            $"Authors: {string.Join(", ", book.Authors)};" +
                            $"Categories: {string.Join(", ", book.Categories)};" +
                            $"Description: {book.Description};";

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