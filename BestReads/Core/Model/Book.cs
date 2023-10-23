using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BestReads.Core;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string Id { get; set; } 
    public string Title { get; set; }
    public List<string> Authors { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Thumbnail { get; set; }
    public string GoogleBooksId { get; set; }
    public string? IndustryIdentifierISBN13 { get; set; }
    public string? IndustryIdentifierISBN10 { get; set; }
    public float[] Embeddings { get; set; } = Array.Empty<float>();
    //Todo: update industry identifier to be in a collection
}