using BestReads.Core;

namespace BestReads.Features.BookRecommendations.Repository;

public interface IBookRepository
{
    Task<Book?> GetByGoogleBooksIdAsync(string googleBooksId);
    Task StoreBookAsync(Book book);
}