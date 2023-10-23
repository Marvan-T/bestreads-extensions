using Azure;
using Azure.AI.OpenAI;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Core;

public interface IOpenAICleint
{
    Task<Response<Embeddings>> GetEmbeddingsAsync(EmbeddingRequest request);
}