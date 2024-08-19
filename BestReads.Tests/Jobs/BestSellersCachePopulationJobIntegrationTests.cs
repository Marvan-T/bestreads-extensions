using BestReads.BackgroundServices;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using BestReads.Infrastructure.ApiClients.NYTimes;
using Quartz.Impl;



public class BestSellersCachePopulationJobIntegrationTests
{
    // [Fact]
    // public async Task BestSellersCachePopulationJob_ShouldBeScheduledAndExecuted()
    // {
    //     var jobKey = new JobKey(nameof(BestSellersCachePopulationJob));
    //     var hostBuilder = new HostBuilder().ConfigureServices(services =>
    //     {
    //         services.AddQuartz(q =>
    //         {
    //             q.AddJob<BestSellersCachePopulationJob>(opts =>
    //                 opts.WithIdentity(jobKey).StoreDurably()
    //             );
    //         });

    //         services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    //     });

    //     using (var host = await hostBuilder.StartAsync())
    //     {
    //         var scheduler = host.Services.GetService<IScheduler>();

    //         await scheduler.TriggerJob(jobKey);
    //     }
    // }

      [Fact]
    public async Task BestSellersCachePopulationJob_ShouldBeScheduledAndExecuted()
    {
        // Arrange
        var jobKey = new JobKey(nameof(BestSellersCachePopulationJob));
        var mockMemoryCache = new Mock<IMemoryCache>();
        var mockLogger = new Mock<ILogger<BestSellersCachePopulationJob>>();
        var mockNYTimesApiClient = new Mock<INYTimesApiClient>();

        var hostBuilder = new HostBuilder().ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton(mockMemoryCache.Object);
            services.AddSingleton(mockLogger.Object);
            services.AddSingleton(mockNYTimesApiClient.Object);

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                
                // Add the job
                q.AddJob<BestSellersCachePopulationJob>(opts => opts
                    .WithIdentity(jobKey)
                    .StoreDurably());
                
                // Add the trigger
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobKey}-trigger")
                    .StartNow());
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        });

        // Act
        using (var host = await hostBuilder.StartAsync())
        {
            // Wait for the host to fully start and the job to potentially execute
            // await Task.Delay(150000); // Adjust this delay as needed
        }

        // Assert
        mockNYTimesApiClient.Verify(x => x.GetCurrentBestSellersList(), Times.Once);
    }
}
