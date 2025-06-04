using System.Net;
using Domain.DTOs.Responses.Problems;
using Domain.Exceptions;

namespace DokWokApi.Middlewares;

public partial class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }
    
    private async Task HandleException(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionAsync(context, validationException);
                break;
            case EntityNotFoundException entityNotFoundException:
                await HandleEntityNotFoundExceptionAsync(context, entityNotFoundException);
                break;
            // case UnauthorizedException unauthorizedException:
            //     await HandleUnauthorizedExceptionAsync(context, unauthorizedException);
            //     break;
            default:
                await HandleDefaultExceptionAsync(context, exception);
                break;
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException)
    {
        LogValidationException(_logger, validationException.Errors);
        const int statusCode = (int)HttpStatusCode.BadRequest;
        context.Response.StatusCode = statusCode;
        var validationProblemDetailsResponse = new ValidationProblemDetailsResponse
        {
            Title = "Validation error",
            Errors = validationException.Errors,
            StatusCode = statusCode,
        };
        await context.Response.WriteAsJsonAsync(validationProblemDetailsResponse);
    }

    private async Task HandleEntityNotFoundExceptionAsync(
        HttpContext context, EntityNotFoundException entityNotFoundException)
    {
        LogEntityNotFoundException(_logger, entityNotFoundException.Message);
        const int statusCode = (int)HttpStatusCode.NotFound;
        context.Response.StatusCode = statusCode;
        var problemDetailsResponse = new ProblemDetailsResponse
        {
            Title = entityNotFoundException.Message,
            StatusCode = statusCode,
        };
        await context.Response.WriteAsJsonAsync(problemDetailsResponse);
    }

    // private async Task HandleUnauthorizedExceptionAsync(
    //     HttpContext context, UnauthorizedException unauthorizedException)
    // {
    //     const int statusCode = (int)HttpStatusCode.Unauthorized;
    //     context.Response.StatusCode = statusCode;
    //     var problemDetailsResponse = new ProblemDetailsResponse
    //     {
    //         Title = unauthorizedException.Message,
    //         StatusCode = statusCode,
    //     };
    //     await context.Response.WriteAsJsonAsync(problemDetailsResponse);
    // }

    private async Task HandleDefaultExceptionAsync(HttpContext context, Exception exception)
    {
        LogUnhandledException(_logger, exception);
        const int statusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.StatusCode = statusCode;
        var problemDetailsResponse = new ProblemDetailsResponse
        {
            Title = "An unexpected server error occured",
            StatusCode = statusCode,
        };
        await context.Response.WriteAsJsonAsync(problemDetailsResponse);
    }
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "ValidationException occured: {errors}")]
    private static partial void LogValidationException(ILogger logger, IDictionary<string, string[]> errors);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "EntityNotFoundException occured: {message}")]
    private static partial void LogEntityNotFoundException(ILogger logger, string message);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "An unhandled exception occurred")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception);
}