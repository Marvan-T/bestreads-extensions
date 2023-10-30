using BestReads.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BestReads.Features.BookRecommendations.Dtos;

public class BookRecommendationDto
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
    public List<IndustryIdentifier> IndustryIdentifiers { get; set; } = new();
}