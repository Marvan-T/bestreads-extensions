using Azure;
using Azure.AI.OpenAI;
using BestReads_Recommendations.Core;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Infrastructure;

public class OpenAIClientService : IOpenAICleint
{
    private readonly OpenAIClient _openAiClient;

    public OpenAIClientService(IConfiguration configuration)
    {
        _openAiClient = new OpenAIClient(configuration["OpenAIKey"]);
    }

    public Task<Response<Embeddings>> GetEmbeddingsAsync(EmbeddingRequest embeddingRequest)
    {
        return _openAiClient.GetEmbeddingsAsync(embeddingRequest.Model, new EmbeddingsOptions(embeddingRequest.Text));
    }
}