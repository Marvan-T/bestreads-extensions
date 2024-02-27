using BestReads.Core;
using BestReads.Core.Data;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads.Features.BookRecommendations.Services.BookSearchService;
using BestReads.Infrastructure;
using BestReads.Infrastructure.ApiClients.NYTimes;
using BestReads.Infrastructure.ApiClients.NYTimes.Converters;
using BestReads.Infrastructure.ApiClients.NYTimes.Handlers;
using BestReads.Infrastructure.AzureSearchClient;
using BestReads.Infrastructure.Data;
using MongoDB.Driver;
using Newtonsoft.Json;
using Refit;

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
        services.AddScoped<IOpenAICleint, OpenAIClientService>();
        services.AddScoped<IAzureSearchClient, AzureSearchClient>();
        services.AddTransient<NyTimesAuthenticationDelegatingHandler>();
    }

    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IBookEmbeddingService, BookEmbeddingService>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookRecommendationService, BookRecommendationService>();
        services.AddScoped<IBookSearchService, BookSearchService>();
    }

    public static void AddDefaultAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program).Assembly);
    }

    public static void SetupMongoDB(this IServiceCollection services, MongoDbSettings mongoDbSettings)
    {
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
            new MongoClient(mongoDbSettings.ConnectionString));
    }

    public static void SetupRefit(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new RefitSettings(new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
        {
            Converters = { new ListDtoConverter() }
        }));
        services.AddRefitClient<INYTimesApiClient>(settings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["NYTimesApi:BaseAddress"]))
            .AddHttpMessageHandler<NyTimesAuthenticationDelegatingHandler>();
    }
}