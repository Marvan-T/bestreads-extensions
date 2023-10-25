using System.Linq.Expressions;
using BestReads.Core.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BestReads.Tests;

public static class ControllerTestHelper
{
    public static ServiceResponse<T> CreateServiceResponse<T>(T data, bool success = true,
        Dictionary<string, string>? errors = null)
    {
        errors ??= new Dictionary<string, string>();
        return new ServiceResponse<T>
        {
            Data = data,
            Success = success,
            Errors = errors
        };
    }

    public static void SetupMockServiceCall<TService, TResponse>(this Mock<TService> mockService,
        Expression<Func<TService, Task<ServiceResponse<TResponse>>>> call,
        ServiceResponse<TResponse> response) where TService : class
    {
        mockService.Setup(call).ReturnsAsync(response);
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
        response.Success.Should().Be(expectedServiceResponse.Success);
        response.Data.Should().BeEquivalentTo(expectedServiceResponse.Data);
        response.Errors.Should().BeEquivalentTo(expectedServiceResponse.Errors);
    }
}