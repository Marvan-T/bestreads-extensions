using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Core;

public interface IOpenAICleint
{
    Task<IReadOnlyList<float>> GetEmbeddingsAsync(EmbeddingRequest request);
}