using Microsoft.EntityFrameworkCore;

namespace BestReads_Recommendations.Core.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Todo: learn a little bit more about owned entities
        modelBuilder.Entity<Book>().OwnsMany(b => b.IndustryIdentifiers);
    }

    public DbSet<Book> Books { get; set; }
}