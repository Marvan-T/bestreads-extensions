using BestReads.Core;
using BestReads.Core.Data;
using BestReads.Features.BestSellers.Services.BestSellersService;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads.Features.BookRecommendations.Services.BookSearchService;
using BestReads.Infrastructure;
using BestReads.Infrastructure.ApiClients.NYTimes;
using BestReads.Infrastructure.ApiClients.NYTimes.Handlers;
using BestReads.Infrastructure.AzureSearchClient;
using BestReads.Infrastructure.Data;
using MongoDB.Driver;
using Refit;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace BestReads;

public static class ApplicationBuilderExtensions
{
    public static void RegisterDatabaseDependencies(this IServiceCollection services)
    {
        services.AddScoped<MongoDbContext>();
        services.AddTransient<IIndexManager, BookIndexManager>();
        services.AddTransient<IndexInitializer>();
    }

    public static void RegisterInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IOpenAIClient, OpenAIClientService>();
        services.AddScoped<IAzureSearchClient, AzureSearchClient>();
        services.AddTransient<NyTimesAuthenticationDelegatingHandler>();
    }

    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IBookEmbeddingService, BookEmbeddingService>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookRecommendationService, BookRecommendationService>();
        services.AddScoped<IBookSearchService, BookSearchService>();
        services.AddScoped<IBestSellersService, BestSellersService>();
    }

    public static void AddDefaultAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program).Assembly);
    }

    public static void SetupMongoDB(
        this IServiceCollection services,
        MongoDbSettings mongoDbSettings
    )
    {
        services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(
            mongoDbSettings.ConnectionString
        ));
    }

    public static void SetupRefit(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRefitClient<INYTimesApiClient>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration["NYTimesApi:BaseAddress"])
            )
            .AddHttpMessageHandler<NyTimesAuthenticationDelegatingHandler>();
    }

    public static void SetupSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog(
            (context, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext();

                if (context.HostingEnvironment.IsProduction())
                {
                    try
                    {
                        configuration.WriteTo.AzureBlobStorage(
                            connectionString: context.Configuration[
                                "AZURE_BLOB_STORAGE_CONNECTION_STRING"
                            ],
                            storageContainerName: "best-reads-logs",
                            storageFileName: "log-{yyyy}/{MM}/{dd}.json",
                            formatter: new JsonFormatter(renderMessage: true, formatProvider: null)
                        );
                    }
                    catch (Exception ex)
                    {
                        // Fallback to stderr for Docker logging
                        configuration.WriteTo.Console(
                            restrictedToMinimumLevel: LogEventLevel.Warning,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                        );

                        // Log the configuration error
                        Console.Error.WriteLine(
                            $"Failed to configure Azure Blob Storage logging: {ex.Message}"
                        );
                    }
                }
                else
                {
                    configuration.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug);
                    configuration.WriteTo.File(
                        formatter: new JsonFormatter(renderMessage: true, formatProvider: null),
                        path: "./logs/log-.json",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        restrictedToMinimumLevel: LogEventLevel.Verbose
                    );
                }
            }
        );
    }
}
