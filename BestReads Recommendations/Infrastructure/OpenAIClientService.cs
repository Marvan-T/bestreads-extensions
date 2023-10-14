using Azure.AI.OpenAI;
using BestReads_Recommendations.Core;

namespace BestReads_Recommendations.Infrastructure;

public class OpenAIClientService : IOpenAICleint
{
    private readonly OpenAIClient _openAiClient;
    
    public OpenAIClientService(IConfiguration configuration)
    {
        _openAiClient = new OpenAIClient(configuration["OpenAIKey"]);
    }
    
}
