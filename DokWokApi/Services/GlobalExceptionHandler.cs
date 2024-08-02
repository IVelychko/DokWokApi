using Domain.Exceptions;
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

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await httpContext.Response.WriteAsJsonAsync(validationException.Errors, cancellationToken);
            return true;
        }

        _logger.LogError(exception, "Server error");
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return true;
    }
}
