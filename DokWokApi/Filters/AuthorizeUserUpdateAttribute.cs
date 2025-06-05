using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Problems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DokWokApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeUserUpdateAttribute : Attribute, IAsyncAuthorizationFilter
{
    private const string UnauthorizedMessage = "The user is not authenticated";
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id");
        var userRoleClaims = context.HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role).ToArray();
        if (userIdClaim is null ||
            !long.TryParse(userIdClaim.Value, out var userIdClaimValue) ||
            userRoleClaims.Length == 0)
        {
            SetUnauthorizedResult(context);
            return;
        }

        if (userRoleClaims.Any(c => c.Value == UserRoles.Admin))
        {
            return;
        }

        var requestBody = await ReadRequestBody(context.HttpContext.Request);
        if (string.IsNullOrWhiteSpace(requestBody))
        {
            return;
        }

        UpdateUserRequest? updateUserRequest;
        try
        {
            updateUserRequest = JsonSerializer.Deserialize<UpdateUserRequest>(requestBody, _jsonOptions);
        }
        catch
        {
            return;
        }
        
        if (updateUserRequest is null)
        {
            return;
        }

        if (userIdClaimValue != updateUserRequest.Id)
        {
            SetForbiddenResult(context);
        }
    }
    
    private static async Task<string?> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var streamReader = new StreamReader(request.Body);
        var body = await streamReader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }
    
    private static void SetUnauthorizedResult(AuthorizationFilterContext context)
    {
        ProblemDetailsResponse problemDetails = new()
        {
            StatusCode = (int)HttpStatusCode.Unauthorized,
            Title = UnauthorizedMessage,
        };
        context.Result = new UnauthorizedObjectResult(problemDetails);
    }
    
    private static void SetForbiddenResult(AuthorizationFilterContext context)
    {
        context.Result = new ForbidResult();
    }
}