namespace BestReads_Recommendations.Core.Data;

public interface IIndexManager<T>
{
    void EnsureIndexes();
}