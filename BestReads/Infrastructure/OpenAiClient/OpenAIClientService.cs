using Azure;
using Azure.AI.OpenAI;
using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Infrastructure;

public class OpenAIClientService : IOpenAICleint
{
    private readonly OpenAIClient _openAiClient;

    public OpenAIClientService(IConfiguration configuration)
    {
        _openAiClient = new OpenAIClient(configuration["Open_AI_Key"]);
    }

    public Task<Response<Embeddings>> GetEmbeddingsAsync(EmbeddingRequest embeddingRequest)
    {
        return _openAiClient.GetEmbeddingsAsync(embeddingRequest.Model, new EmbeddingsOptions(embeddingRequest.Text));
    }
}