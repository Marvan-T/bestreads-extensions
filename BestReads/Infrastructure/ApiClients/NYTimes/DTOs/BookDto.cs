using Newtonsoft.Json;

namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class BookDto
{
    public string Title { get; set; }
    public string Author { get; set; }

    [JsonProperty("primary_isbn10")] public string PrimaryIsbn10 { get; set; }

    [JsonProperty("primary_isbn13")] public string PrimaryIsbn13 { get; set; }

    [JsonProperty("book_image")] public string BookImage { get; set; }
    public int Rank { get; set; }
}