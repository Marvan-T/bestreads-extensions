using System.Text.Json.Serialization;

namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class BookDto
{
    public int Rank { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    [JsonPropertyName("primary_isbn10")] public string PrimaryIsbn10 { get; set; }
    [JsonPropertyName("primary_isbn13")] public string PrimaryIsbn13 { get; set; }
    [JsonPropertyName("book_image")] public string Thumbnail { get; set; }
}