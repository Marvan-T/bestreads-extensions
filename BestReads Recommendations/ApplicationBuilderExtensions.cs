using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Data;
using BestReads_Recommendations.Features.BookRecommendations.Repository;
using BestReads_Recommendations.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads_Recommendations.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads_Recommendations.Infrastructure;
using BestReads_Recommendations.Infrastructure.Data;
using MongoDB.Driver;

namespace BestReads_Recommendations;

public static class ApplicationBuilderExtensions
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IOpenAICleint, OpenAIClientService>();
        services.AddScoped<IBookEmbeddingService, BookEmbeddingService>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookRecommendationService, BookRecommendationService>();
    }

    public static void RegisterDatabaseDependencies(this IServiceCollection services)
    {
        services.AddScoped<MongoDbContext>();
        services.AddTransient<IIndexManager<Book>, BookIndexManager>();
        services.AddTransient<IndexInitializer>();
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
}