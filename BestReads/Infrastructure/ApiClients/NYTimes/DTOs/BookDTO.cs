using Newtonsoft.Json;

namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class BookDTO
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string PrimaryIsbn10 { get; set; }
    public string PrimaryIsbn13 { get; set; }

    [JsonProperty("book_image")] public string BookImage { get; set; }

    public int Rank { get; set; }
}