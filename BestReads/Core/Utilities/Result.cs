namespace BestReads.Core.Utilities;

public class Result<T>
{
    private Result(T value, bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new ArgumentException("Invalid error state", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public bool IsSuccess { get; }
    public Error Error { get; }
    public T Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, Error.None);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(default!, false, error);
    }
}