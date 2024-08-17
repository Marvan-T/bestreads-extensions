using BestReads.Infrastructure.ApiClients.NYTimes;
using Microsoft.Extensions.Caching.Memory;
using Quartz;

namespace BestReads.BackgroundServices;

[DisallowConcurrentExecution]
public class BestSellersCachePopulationJob : IJob
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<BestSellersCachePopulationJob> _logger;
    private readonly INYTimesApiClient _nyTimesApiClient;

    public BestSellersCachePopulationJob(
        IMemoryCache memoryCache,
        ILogger<BestSellersCachePopulationJob> logger,
        INYTimesApiClient nyTimesApiClient
    )
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _nyTimesApiClient = nyTimesApiClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Executing job:{context.JobDetail}");

        if (!context.CancellationToken.IsCancellationRequested)
        {
            var bestSellers = await _nyTimesApiClient.GetCurrentBestSellersList();
            _memoryCache.Set($"BestSellers-{DateTime.Today.ToShortDateString()}", bestSellers);

            var cachedBestSellers = _memoryCache.Get(
                $"BestSellers-{DateTime.Today.ToShortDateString()}"
            );

            if (cachedBestSellers is not null)
            {
                _logger.LogInformation(
                    $"Best sellers for {DateTime.Today.ToShortDateString} cached successfully:{cachedBestSellers}"
                );
            }
            else
            {
                _logger.LogError($"Cache population unsuccessful");
            }
        }
    }
}
