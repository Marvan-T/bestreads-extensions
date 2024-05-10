using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Core;

public interface IOpenAIClient
{
    Task<IReadOnlyList<float>> GetEmbeddingsAsync(EmbeddingRequest request);
}