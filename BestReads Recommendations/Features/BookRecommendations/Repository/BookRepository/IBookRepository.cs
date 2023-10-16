using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Repository;

namespace BestReads_Recommendations.Features.BookRecommendations.Repository;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByGoogleBooksIdAsync(string googleBooksId);
}