using Microsoft.EntityFrameworkCore;

namespace BestReads_Recommendations.Core.Data;

public class DataContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookConfiguration());
    }
}