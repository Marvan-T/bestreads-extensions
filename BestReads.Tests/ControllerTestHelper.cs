using System.Linq.Expressions;
using BestReads.Core.Responses;
using BestReads.Core.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BestReads.Tests;

public static class ControllerTestHelper
{
    public static ServiceResponse<T> CreateServiceResponseFromResult<T>(Result<T> result)
    {
        return result.IsSuccess ? ServiceResponse<T>.Success(result.Data) : ServiceResponse<T>.Failure(result.Error);
    }

    public static Result<T> CreateResult<T>(T data, bool success = true, Error error = null)
    {
        error = error ?? Error.None;
        return success ? Result<T>.Success(data) : Result<T>.Failure(error);
    }

    public static void SetupMockServiceCall<TService, TResult>(this Mock<TService> mockService,
        Expression<Func<TService, Task<Result<TResult>>>> call,
        Result<TResult> returningResult) where TService : class
    {
        mockService.Setup(call).ReturnsAsync(returningResult);
    }

    public static void CheckResponse<T>(ActionResult<ServiceResponse<T>> result, Type expectedObjectResultType,
        ServiceResponse<T> expectedServiceResponse)
    {
        // Asserting the expected status code
        Assert.IsType(expectedObjectResultType, result.Result);

        // is the response a type of ServiceResponse
        var objectResult = (ObjectResult)result.Result;
        var response = Assert.IsType<ServiceResponse<T>>(objectResult.Value);

        // Asserting the content of service response
        response.Result.IsSuccess.Should().Be(expectedServiceResponse.Result.IsSuccess);
        response.Result.Data.Should().BeEquivalentTo(expectedServiceResponse.Result.Data);
        response.Result.Error.Should().Be(expectedServiceResponse.Result.Error);
    }
}