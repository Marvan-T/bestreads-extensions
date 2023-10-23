using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Data;

namespace BestReads_Recommendations.Infrastructure.Data;

public class IndexInitializer
{
    private readonly IIndexManager<Book> _bookIndexManager;

    public IndexInitializer(IIndexManager<Book> bookIndexManager)
    {
        _bookIndexManager = bookIndexManager;
    }

    public void Initialize()
    {
        _bookIndexManager.EnsureIndexes();
    }
}