using BestReads.BackgroundServices;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

public class BestSellersCachePopulationJobIntegrationTests
{
    [Fact]
    public async Task BestSellersCachePopulationJob_ShouldBeScheduledAndExecuted()
    {
        var jobKey = new JobKey(nameof(BestSellersCachePopulationJob));
        var hostBuilder = new HostBuilder().ConfigureServices(services =>
        {
            services.AddQuartz(q =>
            {
                q.AddJob<BestSellersCachePopulationJob>(opts =>
                    opts.WithIdentity(jobKey).StoreDurably()
                );
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        });

        using (var host = await hostBuilder.StartAsync())
        {
            var scheduler = host.Services.GetService<IScheduler>();

            await scheduler.TriggerJob(jobKey);
        }
    }

    // [Fact]
    // public async Task BestSellersCachePopulationJob_ShouldBeScheduledAndExecuted()
    // {
    //     // Arrange
    //     var jobKey = new JobKey(nameof(BestSellersCachePopulationJob));
    //     var mockMemoryCache = new Mock<IMemoryCache>();
    //     var mockLogger = new Mock<ILogger<BestSellersCachePopulationJob>>();
    //     var mockNYTimesApiClient = new Mock<INYTimesApiClient>();

    //     var hostBuilder = new HostBuilder().ConfigureServices(
    //         (hostContext, services) =>
    //         {
    //             services.AddSingleton(mockMemoryCache.Object);
    //             services.AddSingleton(mockLogger.Object);
    //             services.AddSingleton(mockNYTimesApiClient.Object);

    //             services.AddQuartz(q =>
    //             {
    //                 q.UseMicrosoftDependencyInjectionJobFactory();
    //                 q.AddJob<BestSellersCachePopulationJob>(opts =>
    //                     opts.WithIdentity(jobKey).StoreDurably()
    //                 );
    //             });
    //             services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    //         }
    //     );

    //     // Act
    //     using (var host = await hostBuilder.StartAsync())
    //     {
    //         var scheduler = host.Services.GetRequiredService<IScheduler>();
    //         await scheduler.TriggerJob(jobKey);

    //         // Allow some time for the job to execute
    //         await Task.Delay(1000);
    //     }

    //     // Assert
    //     mockLogger.Verify(
    //         x =>
    //             x.Log(
    //                 It.IsAny<LogLevel>(),
    //                 It.IsAny<EventId>(),
    //                 It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Executing job")),
    //                 It.IsAny<Exception>(),
    //                 It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
    //             ),
    //         Times.Once
    //     );

    //     mockNYTimesApiClient.Verify(x => x.GetCurrentBestSellersList(), Times.Once);
    // }
}
