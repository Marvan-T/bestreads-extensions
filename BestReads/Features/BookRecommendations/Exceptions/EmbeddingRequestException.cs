using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Exceptions;

public class EmbeddingRequestException : InvalidOperationException
{
    private const string DefaultMessage = "No embedding data was returned from OpenAI.";

    public EmbeddingRequestException(EmbeddingRequest embeddingRequest)
        : base(DefaultMessage)
    {
        EmbeddingRequest = embeddingRequest;
    }

    public EmbeddingRequest EmbeddingRequest { get; }
}