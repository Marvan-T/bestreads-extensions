namespace BestReads_Recommendations.Core;

public class Book : IEntityWithId
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public List<string> Categories { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Thumbnail { get; set; }
    public string GoogleBooksId { get; set; }
    public List<IndustryIdentifier> IndustryIdentifiers { get; set; }
    public float[] Embeddings { get; set; }
}