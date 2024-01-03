using BestReads.Core;
using BestReads.Infrastructure.Data;
using MongoDB.Driver;

namespace BestReads.Features.BookRecommendations.Repository;

public class BookRepository(MongoDbContext mongoDbContext) : IBookRepository
{
    private readonly IMongoCollection<Book> _books = mongoDbContext.Books;

    public Task<Book?> GetByGoogleBooksIdAsync(string googleBooksId)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.GoogleBooksId, googleBooksId);
        return _books.Find(filter).FirstOrDefaultAsync();
    }

    public Task StoreBookAsync(Book book)
    {
        return _books.InsertOneAsync(book);
    }
}