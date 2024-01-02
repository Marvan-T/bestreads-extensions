using BestReads.Core.Data;

namespace BestReads.Infrastructure.Data;

public class IndexInitializer
{
    private readonly IIndexManager _bookIndexManager;

    public IndexInitializer(IIndexManager bookIndexManager)
    {
        _bookIndexManager = bookIndexManager;
    }

    public void Initialize()
    {
        _bookIndexManager.EnsureIndexes();
    }
}