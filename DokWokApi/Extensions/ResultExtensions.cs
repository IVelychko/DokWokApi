using Application.Operations;
using Application.Operations.User;
using Domain.Errors.Base;
using Domain.Models;
using Domain.ResultType;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Extensions;

public static class ResultExtensions
{
    // IActionResult
    public static IActionResult ToOkActionResult(this Result<AuthorizedUserResponse> result, HttpContext context)
    {
        if (result.IsFaulted)
        {
            return GetActionResultFromError(result.Error);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken!.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken!.Token, cookieOptions);
        return new OkObjectResult(user);
    }

    public static IActionResult ToOkActionResult<TResponse>(this Result<TResponse> result)
    {
        return result.Match(response => new OkObjectResult(response), GetActionResultFromError);
    }

    public static IActionResult ToOkPasswordUpdateActionResult(this Result<bool> result)
    {
        return result.Match(isUpdated => new OkResult(), GetActionResultFromError);
    }

    public static IActionResult ToOkIsTakenActionResult(this Result<bool> result)
    {
        return result.Match<IActionResult>(isTaken => new OkObjectResult(new { isTaken }), error =>
        {
            if (error is BadRequestError badRequestError)
            {
                return new BadRequestObjectResult(new ProblemDetailsModel
                {
                    Errors = badRequestError.Errors,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Bad Request"
                });
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        });
    }

    public static IActionResult ToCreatedAtActionResult(this Result<AuthorizedUserResponse> result, HttpContext context, string actionName, string controllerName)
    {
        if (result.IsFaulted)
        {
            return GetActionResultFromError(result.Error);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken!.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken!.Token, cookieOptions);
        return new CreatedAtActionResult(actionName, controllerName, new { id = user.Id }, user);
    }

    public static IActionResult ToCreatedAtActionResult<TResponse, TKey>(this Result<TResponse> result, string actionName, string controllerName) where TResponse : BaseResponse<TKey>
    {
        return result.Match(response => new CreatedAtActionResult(actionName, controllerName, new { id = response.Id }, response),
            GetActionResultFromError);
    }

    private static IActionResult GetActionResultFromError(Error error)
    {
        if (error is BadRequestError badRequestError)
        {
            return new BadRequestObjectResult(new ProblemDetailsModel
            {
                Errors = badRequestError.Errors,
                StatusCode = StatusCodes.Status400BadRequest,
                Title = "Bad Request"
            });
        }
        else if (error is NotFoundError notFoundError)
        {
            return new NotFoundObjectResult(new ProblemDetailsModel
            {
                Errors = notFoundError.Errors,
                StatusCode = StatusCodes.Status404NotFound,
                Title = "Not Found"
            });
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    // IResult
    public static IResult ToOkResult(this Result<AuthorizedUserResponse> result, HttpContext context)
    {
        if (result.IsFaulted)
        {
            return GetResultFromError(result.Error);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken!.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken!.Token, cookieOptions);
        return Results.Ok(user);
    }

    public static IResult ToOkResult<TResponse>(this Result<TResponse> result)
    {
        return result.Match(response => Results.Ok(response), GetResultFromError);
    }

    public static IResult ToOkPasswordUpdateResult(this Result<bool> result)
    {
        return result.Match(isUpdated => Results.Ok(), GetResultFromError);
    }

    public static IResult ToOkIsTakenResult(this Result<bool> result)
    {
        return result.Match(isTaken => Results.Ok(new IsTakenResponse(isTaken)), error =>
        {
            if (error is BadRequestError badRequestError)
            {
                return Results.BadRequest(new ProblemDetailsModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Errors = badRequestError.Errors
                });
            }

            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        });
    }

    public static IResult ToCreatedAtRouteResult(this Result<AuthorizedUserResponse> result, HttpContext context, string routeName)
    {
        if (result.IsFaulted)
        {
            return GetResultFromError(result.Error);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken!.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken!.Token, cookieOptions);
        return Results.CreatedAtRoute(routeName, new { id = user.Id }, user);
    }

    public static IResult ToCreatedAtRouteResult<TResponse, TKey>(this Result<TResponse> result, string routeName) where TResponse : BaseResponse<TKey>
    {
        return result.Match(response => Results.CreatedAtRoute(routeName, new { id = response.Id }, response),
            GetResultFromError);
    }

    private static IResult GetResultFromError(Error error)
    {
        if (error is BadRequestError badRequestError)
        {
            return Results.BadRequest(new ProblemDetailsModel
            {
                Title = "Bad Request",
                StatusCode = StatusCodes.Status400BadRequest,
                Errors = badRequestError.Errors
            });
        }
        else if (error is NotFoundError notFoundError)
        {
            return Results.NotFound(new ProblemDetailsModel
            {
                Title = "Not Found",
                StatusCode = StatusCodes.Status404NotFound,
                Errors = notFoundError.Errors
            });
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
}
