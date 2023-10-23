using BestReads_Recommendations.Core;
using MongoDB.Driver;

namespace BestReads_Recommendations.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, MongoDbSettings mongoDbSettings)
    {
        _database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
    }

    public IMongoCollection<Book> Books => _database.GetCollection<Book>("Books");
}