using BestReads_Recommendations.Core;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Features.BookRecommendations.Services.BookEmbeddingService;

public interface IBookEmbeddingService
{
    EmbeddingRequest ConstructEmbeddingRequest(Book book);
    Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request);
}