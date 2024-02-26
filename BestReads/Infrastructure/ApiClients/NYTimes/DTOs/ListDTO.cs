namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class ListDTO
{
    public int ListId { get; set; }
    public string ListName { get; set; }
    public string DisplayName { get; set; }
    public List<BookDTO> Books { get; set; }
}