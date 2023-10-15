using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Data;
using BestReads_Recommendations.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads_Recommendations.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BestReads_Recommendations;

public static class ApplicationBuilderExtensions
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IOpenAICleint, OpenAIClientService>();
        services.AddScoped<IBookEmbeddingService, BookEmbeddingService>();
    }
    
    public static void AddDefaultDbContext(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
    }
}