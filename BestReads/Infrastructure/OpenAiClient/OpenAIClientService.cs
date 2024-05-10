using Azure;
using Azure.AI.OpenAI;
using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Exceptions;

namespace BestReads.Infrastructure;

public class OpenAIClientService : IOpenAIClient
{
    private readonly OpenAIClient _openAiClient;

    public OpenAIClientService(IConfiguration configuration)
    {
        _openAiClient = new OpenAIClient(configuration["Open_AI_Key"]);
    }

    public async Task<IReadOnlyList<float>> GetEmbeddingsAsync(EmbeddingRequest request)
    {
        EmbeddingsOptions embeddingsOptions = new()
        {
            DeploymentName = request.Model,
            Input = { request.Text }
        };
        Response<Embeddings> response = await _openAiClient.GetEmbeddingsAsync(embeddingsOptions);

        if (response.Value.Data.Any()) return response.Value.Data[0].Embedding.ToArray().ToList();

        throw new EmbeddingRequestException(request);
    }
}