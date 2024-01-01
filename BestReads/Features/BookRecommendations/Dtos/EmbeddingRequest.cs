namespace BestReads.Features.BookRecommendations.Dtos;

public class EmbeddingRequest
{
    public string Model { get; set; } = "text-embedding-ada-002";
    public string Text { get; set; } = string.Empty;
}