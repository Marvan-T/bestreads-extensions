namespace BestReads_Recommendations.Core.Repository;

public interface IRepository<T> where T : class
{
    void Add(T entity);
    Task<T> GetByIdAsync(int id);
    Task SaveChangesAsync();
}