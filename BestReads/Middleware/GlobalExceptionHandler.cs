using BestReads.Core.Responses;
using BestReads.Core.Utilities;
using Microsoft.AspNetCore.Diagnostics;

namespace BestReads;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public static readonly Error ServerError =
        new(
            "internal_server_error",
            "An internal server error has occurred. Please try again later."
        );

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);

        var response = ServiceResponse<bool>.Failure(ServerError);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}