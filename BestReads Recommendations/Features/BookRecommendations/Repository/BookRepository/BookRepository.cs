using BestReads_Recommendations.Core;
using BestReads_Recommendations.Infrastructure.Data;
using MongoDB.Driver;

namespace BestReads_Recommendations.Features.BookRecommendations.Repository;

public class BookRepository : IBookRepository
{
    private readonly IMongoCollection<Book> _books;

    public BookRepository(MongoDbContext mongoDbContext)
    {
        _books = mongoDbContext.Books;
    }

    public Task<Book> GetByGoogleBooksIdAsync(string googleBooksId)
    {
        var filter = Builders<Book>.Filter.Eq(b => b.GoogleBooksId, googleBooksId);
        return _books.Find(filter).FirstOrDefaultAsync();
    }

    public Task StoreBookAsync(Book book)
    {
        return _books.InsertOneAsync(book);
    }
}