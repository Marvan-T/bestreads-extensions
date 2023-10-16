namespace BestReads_Recommendations.Core.Responses;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public string Message
    {
        get => string.Join("; ", Errors);
        set => Errors.Add(value);
    }
}