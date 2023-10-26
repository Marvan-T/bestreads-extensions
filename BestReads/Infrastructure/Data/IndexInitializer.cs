using BestReads.Core;
using BestReads.Core.Data;

namespace BestReads.Infrastructure.Data;

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