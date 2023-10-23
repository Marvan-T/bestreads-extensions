using BestReads_Recommendations.Core;

namespace BestReads_Recommendations.Features.BookRecommendations.Repository;

public interface IBookRepository
{
    Task<Book> GetByGoogleBooksIdAsync(string googleBooksId);
    Task StoreBookAsync(Book book);
}