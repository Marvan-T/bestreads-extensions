using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Newtonsoft.Json;

namespace BestReads.Core;

public class Book : IEntityWithId
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? AuthorsJson { get; set; }
    public string? CategoriesJson { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Thumbnail { get; set; }
    public string GoogleBooksId { get; set; }
    public string? IndustryIdentifierISBN13 { get; set; }
    public string? IndustryIdentifierISBN10 { get; set; }
    public string EmbeddingsAsJson { get; set; } 
    [NotMapped]
    public float[] Embeddings
    {
        get => JsonToFloatArray(EmbeddingsAsJson);
        set => EmbeddingsAsJson = FloatArrayToJson(value);
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

    private static float[] JsonToFloatArray(string json)
    {
        var stringArray = JsonConvert.DeserializeObject<string[]>(json);
        return Array.ConvertAll(stringArray, s => float.Parse(s, CultureInfo.InvariantCulture));
    }

    private static string FloatArrayToJson(float[] floatArray)
    {
        var stringArray = Array.ConvertAll(floatArray, f => f.ToString(CultureInfo.InvariantCulture));
        return JsonConvert.SerializeObject(stringArray);
    }
    
}