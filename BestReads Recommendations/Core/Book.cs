namespace BestReads_Recommendations.Core;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public List<string> Categories { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishedDate { get; set; }
    public string ISBN { get; set; }
    public float[] Embeddings { get; set; }
}