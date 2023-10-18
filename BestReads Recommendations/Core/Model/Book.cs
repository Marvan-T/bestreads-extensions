using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BestReads_Recommendations.Core;

public class Book : IEntityWithId
{
    public string Title { get; set; }
    public string AuthorsJson { get; set; }
    public string CategoriesJson { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Thumbnail { get; set; }
    public string GoogleBooksId { get; set; }
    public string? IndustryIdentifierISBN13 { get; set; }
    public string? IndustryIdentifierISBN10 { get; set; }
    public byte[] EmbeddingsBinary { get; set; }

    [NotMapped]
    public float[] Embeddings
    {
        get => ToFloatArray(EmbeddingsBinary);
        set => EmbeddingsBinary = ToByteArray(value);
    }

    [NotMapped]
    public List<string>? Authors
    {
        get => JsonConvert.DeserializeObject<List<string>>(AuthorsJson);
        set => AuthorsJson = JsonConvert.SerializeObject(value);
    }

    [NotMapped]
    public List<string>? Categories
    {
        get => JsonConvert.DeserializeObject<List<string>>(CategoriesJson);
        set => CategoriesJson = JsonConvert.SerializeObject(value);
    }

    public int Id { get; set; }

    private static float[] ToFloatArray(byte[] byteArray)
    {
        var floatArray = new float[byteArray.Length / sizeof(float)];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    private static byte[] ToByteArray(float[] floatArray)
    {
        var byteArray = new byte[floatArray.Length * sizeof(float)];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }
}