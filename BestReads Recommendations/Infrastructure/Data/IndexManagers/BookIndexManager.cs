using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Data;
using MongoDB.Driver;

namespace BestReads_Recommendations.Infrastructure.Data;

public class BookIndexManager : IIndexManager<Book>
{
    private readonly IMongoCollection<Book> _books;

    public BookIndexManager(MongoDbContext mongoDbContext)
    {
        _books = mongoDbContext.Books;
    }

    public void EnsureIndexes()
    {
        var builder = Builders<Book>.IndexKeys;
        var googleBooksIdIndexModel = new CreateIndexModel<Book>(builder.Ascending(b => b.GoogleBooksId));
        _books.Indexes.CreateOne(googleBooksIdIndexModel);
    }
}