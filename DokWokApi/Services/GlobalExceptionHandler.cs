using Domain.Exceptions;
using Domain.Models;
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
            int statusCode = (int)HttpStatusCode.BadRequest;
            httpContext.Response.StatusCode = statusCode;
            ProblemDetailsModel problem = new() { StatusCode = statusCode, Title = "Bad Request", Errors = validationException.Errors };
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
        else if (exception is NotFoundException notFoundException)
        {
            int statusCode = (int)HttpStatusCode.NotFound;
            httpContext.Response.StatusCode = statusCode;
            ProblemDetailsModel problem = new() { StatusCode = statusCode, Title = "Not Found", Errors = notFoundException.Errors };
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        _logger.LogError(exception, "Server error");
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return true;
    }
}
