namespace BestReads.Core.Responses;

public class ServiceResponse<T>
{
    /// <summary>
    ///     The main data payload of the response.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    ///     Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    ///     Map of error codes/types to error messages.
    /// </summary>
    public Dictionary<string, string> Errors { get; set; } = new();


    /// <summary>
    ///     Retrieves a concatenated error message from the Errors map.
    /// </summary>
    public string ErrorMessage => string.Join("; ", Errors.Select(kv => $"{kv.Key}: {kv.Value}"));

    /// <summary>
    ///     Adds a new error to the Errors map.
    /// </summary>
    /// <param name="errorKey">The error key (e.g., code or type).</param>
    /// <param name="errorMessage">The error message associated with the key.</param>
    public void AddError(string errorKey, string errorMessage, bool overwrite = false)
    {
        if (overwrite || !Errors.ContainsKey(errorKey))
            Errors[errorKey] = errorMessage;
        else
            Errors[errorKey] += "; " + errorMessage;
    }
}