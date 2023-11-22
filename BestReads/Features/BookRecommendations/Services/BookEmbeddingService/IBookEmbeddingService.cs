using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookEmbeddingService;

public interface IBookEmbeddingService
{
    Result<EmbeddingRequest> ConstructEmbeddingRequest(GetBookRecommendationsDto bookRecommendationsDto);
    Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request);
}