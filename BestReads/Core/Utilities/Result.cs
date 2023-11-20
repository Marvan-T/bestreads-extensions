namespace BestReads.Core.Utilities;

public class Result<T>
{
    private Result(T data, bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new ArgumentException("Invalid error state", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
        Data = data;
    }

    public bool IsSuccess { get; }
    public Error Error { get; }
    public T Data { get; }

    public static Result<T> Success(T data)
    {
        return new Result<T>(data, true, Error.None);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(default!, false, error);
    }
}