using Azure.AI.OpenAI;
using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Exceptions;

namespace BestReads.Infrastructure;

public class OpenAIClientService : IOpenAICleint
{
    private readonly OpenAIClient _openAiClient;

    public OpenAIClientService(IConfiguration configuration)
    {
        _openAiClient = new OpenAIClient(configuration["Open_AI_Key"]);
    }

    public async Task<IReadOnlyList<float>> GetEmbeddingsAsync(EmbeddingRequest embeddingRequest)
    {
        var response =
            await _openAiClient.GetEmbeddingsAsync(embeddingRequest.Model,
                new EmbeddingsOptions(embeddingRequest.Text));

        if (response.Value.Data.Any()) return response.Value.Data[0].Embedding;

        throw new EmbeddingRequestException(embeddingRequest);
    }
}