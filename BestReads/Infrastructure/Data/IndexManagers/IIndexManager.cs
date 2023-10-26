namespace BestReads.Core.Data;

public interface IIndexManager<T>
{
    void EnsureIndexes();
}