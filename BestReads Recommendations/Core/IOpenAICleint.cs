using Azure;
using Azure.AI.OpenAI;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Core;

public interface IOpenAICleint
{
    Task<Response<Embeddings>> GetEmbeddingsAsync(EmbeddingRequest request);
}