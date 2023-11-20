using BestReads.Core.Utilities;

namespace BestReads.Core.Responses;

public static class ResultExtensions
{
    public static ServiceResponse<T> Match<T>(
        this Result<T> result,
        Func<T, ServiceResponse<T>> onSuccess,
        Func<Error, ServiceResponse<T>> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Data) : onFailure(result.Error);
    }
}