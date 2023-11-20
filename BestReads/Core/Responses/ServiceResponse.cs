using BestReads.Core.Utilities;

namespace BestReads.Core.Responses;

public class ServiceResponse<T>
{
    private ServiceResponse(Result<T> result)
    {
        Result = result;
    }

    public Result<T> Result { get; }

    public T Data => Result.Value;

    public static ServiceResponse<T> Success(T data)
    {
        return new ServiceResponse<T>(Result<T>.Success(data));
    }

    public static ServiceResponse<T> Failure(Error error)
    {
        return new ServiceResponse<T>(Result<T>.Failure(error));
    }
}