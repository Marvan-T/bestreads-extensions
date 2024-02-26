namespace BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

public class ResultDTO
{
    public string Bestsellers_Date { get; set; }
    public List<ListDTO> Lists { get; set; }
}