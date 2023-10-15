namespace BestReads_Recommendations.Core.Repository;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task SaveChangesAsync();
}