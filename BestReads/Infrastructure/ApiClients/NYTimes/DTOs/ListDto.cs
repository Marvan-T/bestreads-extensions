namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class ListDto
{
    public int ListId { get; set; }
    public string ListName { get; set; }
    public string DisplayName { get; set; }
    public List<BookDto> Books { get; set; }
}