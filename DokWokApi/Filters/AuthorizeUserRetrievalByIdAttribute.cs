using System.Net;
using System.Security.Claims;
using Domain.Constants;
using Domain.DTOs.Responses.Problems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DokWokApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeUserRetrievalByIdAttribute : Attribute, IAuthorizationFilter
{
    private const string UnauthorizedMessage = "The user is not authenticated";
    private const string NotFoundMessage = "The user was not found";
    
    public void OnAuthorization(AuthorizationFilterContext context)
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
        
        var path = context.HttpContext.Request.Path.ToString();
        var userIdPathVariable = path.Split('/')[^1];
        if (!long.TryParse(userIdPathVariable, out var userIdToRetrieve))
        {
            SetNotFoundResult(context);
            return;
        }

        if (userIdClaimValue != userIdToRetrieve)
        {
            SetForbiddenResult(context);
        }
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

    private static void SetNotFoundResult(AuthorizationFilterContext context)
    {
        ProblemDetailsResponse problemDetails = new()
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Title = NotFoundMessage
        };
        context.Result = new NotFoundObjectResult(problemDetails);
    }

    private static void SetForbiddenResult(AuthorizationFilterContext context)
    {
        context.Result = new ForbidResult();
    }
}