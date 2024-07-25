using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace DokWokApi.Services;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Server error");
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return ValueTask.FromResult(true);
    }
}
