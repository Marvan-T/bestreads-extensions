using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;
using Refit;

namespace BestReads.Infrastructure.ApiClients.NYTimes;

public interface INYTimesApiClient
{
    [Get("/svc/books/v3/lists/overview.json")]
    Task<BestSellerListDTO> GetCurrentBestSellersList();
}