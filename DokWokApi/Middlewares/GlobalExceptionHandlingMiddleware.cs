using System.Net;

namespace DokWokApi.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
