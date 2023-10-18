namespace BestReads_Recommendations.Core.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasIndex(b => b.GoogleBooksId).HasDatabaseName("Index_GoogleBooksId");
    }
}
