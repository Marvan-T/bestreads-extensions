using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookEmbeddingService;

public interface IBookEmbeddingService
{
    EmbeddingRequest ConstructEmbeddingRequest(GetBookRecommendationsDto bookRecommendationsDto);
    Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request);
}