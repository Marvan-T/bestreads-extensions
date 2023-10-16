using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Features.BookRecommendations.Services.BookEmbeddingService;

public interface IBookEmbeddingService
{
    EmbeddingRequest ConstructEmbeddingRequest(GetBookRecommendationsDto bookRecommendationsDto);
    Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request);
}