namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class ResultDto
{
    public string Bestsellers_Date { get; set; }
    public List<ListDto> Lists { get; set; }
}