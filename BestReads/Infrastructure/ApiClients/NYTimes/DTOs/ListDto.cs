using System.Text.Json.Serialization;

namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class ListDto
{
    [JsonPropertyName("list_id")] public int ListId { get; set; }
    [JsonPropertyName("list_name")] public string ListName { get; set; }
    [JsonPropertyName("display_name")] public string DisplayName { get; set; }
    public List<BookDto> Books { get; set; }
}