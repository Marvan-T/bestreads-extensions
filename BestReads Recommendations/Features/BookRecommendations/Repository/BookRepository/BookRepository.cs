using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Data;
using BestReads_Recommendations.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace BestReads_Recommendations.Features.BookRecommendations.Repository;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(DataContext dataContext) : base(dataContext)
    {
    }

    public Task<Book?> GetByGoogleBooksIdAsync(string googleBooksId)
    {
        return _dataContext.Books
            .FirstOrDefaultAsync(b => b.GoogleBooksId == googleBooksId);
    }
}