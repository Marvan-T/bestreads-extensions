namespace BestReads.Core.Responses;

public class ServiceResponse<T>
{
    /// <summary>
    /// The main data payload of the response.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// List of error messages, if any.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Adds a new error message to the Errors list.
    /// </summary>
    /// <param name="errorMessage">The error message to add.</param>
    public void AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
    }

    /// <summary>
    /// Retrieves a concatenated error message from the Errors list.
    /// </summary>
    public string ErrorMessage => string.Join("; ", Errors);
}
