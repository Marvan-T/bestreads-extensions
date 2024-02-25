using Refit;

namespace BestReads.Infrastructure.ApiClients.NYTimes;

public interface INYTimesApiClient
{
    [Get("/svc/books/v3/lists/overview.json")]
    Task<HttpResponseMessage> GetCurrentBestSellersList();
}