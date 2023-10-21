using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Repository;

namespace BestReads_Recommendations.Features.BookRecommendations.Repository;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public Task<Book?> GetByGoogleBooksIdAsync(string googleBooksId)
    {
        throw new NotImplementedException();
    }
}