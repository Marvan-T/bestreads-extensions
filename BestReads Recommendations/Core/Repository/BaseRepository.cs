using BestReads_Recommendations.Core.Data;

namespace BestReads_Recommendations.Core.Repository;

public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntityWithId
{
    private readonly DataContext _dataContext;

    public BaseRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _dataContext.Set<T>().FindAsync(id);
    }

    public virtual async Task SaveChangesAsync()
    {
        await _dataContext.SaveChangesAsync();
    }
}