namespace BestReads_Recommendations.Core.Repository;

public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntityWithId
{
    public void Add(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}