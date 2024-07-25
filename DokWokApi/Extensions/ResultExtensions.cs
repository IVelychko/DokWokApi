using DokWokApi.BLL.Extensions;
using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using DokWokApi.Models.ShoppingCart;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Extensions;

public static class ResultExtensions
{
    // IActionResult

    public static IActionResult ToOkActionResult(this Result<AuthorizedUserModel> result, HttpContext context)
    {
        if (result.IsFaulted)
        {
            return GetActionResultFromError(result.Exception);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken.Token, cookieOptions);
        return new OkObjectResult(user.ToAuthorizedResponseModel());
    }

    public static IActionResult ToOkActionResult<TModel>(this Result<TModel> result)
    {
        return result.Match(model => new OkObjectResult(model), GetActionResultFromError);
    }

    public static IActionResult ToOkActionResult(this Result<Cart?> result)
    {
        return result.Match(cart =>
        {
            return cart is not null ? new OkObjectResult(cart) 
                : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }, GetActionResultFromError);
    }

    public static IActionResult ToOkPasswordUpdateActionResult(this Result<bool> result)
    {
        return result.Match(isUpdated => new OkResult(), GetActionResultFromError);
    }

    public static IActionResult ToOkIsTakenActionResult(this Result<bool> result)
    {
        return result.Match<IActionResult>(isTaken => new OkObjectResult(new { isTaken }), e =>
        {
            if (e is ValidationException validationException)
            {
                return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        });
    }

    public static IActionResult ToCreatedAtActionActionResult(this Result<UserModel> result, string actionName, string controllerName)
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model), 
            GetActionResultFromError);
    }

    public static IActionResult ToCreatedAtActionActionResult(this Result<AuthorizedUserModel> result, HttpContext context, string actionName, string controllerName)
    {
        if (result.IsFaulted)
        {
            return GetActionResultFromError(result.Exception);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken.Token, cookieOptions);
        return new CreatedAtActionResult(actionName, controllerName, new { id = user.Id }, user.ToAuthorizedResponseModel());
    }

    public static IActionResult ToCreatedAtActionActionResult<TModel>(this Result<TModel> result, string actionName, string controllerName) where TModel : BaseModel
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model),
            GetActionResultFromError);
    }

    private static IActionResult GetActionResultFromError(Exception e)
    {
        if (e is ValidationException validationException)
        {
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is NotFoundException)
        {
            return new NotFoundResult();
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    // IResult

    public static IResult ToOkResult(this Result<AuthorizedUserModel> result, HttpContext context)
    {
        if (result.IsFaulted)
        {
            return GetResultFromError(result.Exception);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken.Token, cookieOptions);
        return Results.Ok(user.ToAuthorizedResponseModel());
    }

    public static IResult ToOkResult<TModel>(this Result<TModel> result)
    {
        return result.Match(model => Results.Ok(model), GetResultFromError);
    }

    public static IResult ToOkResult(this Result<Cart?> result)
    {
        return result.Match(cart =>
        {
            return cart is not null ? Results.Ok(cart)
                : Results.StatusCode(StatusCodes.Status500InternalServerError);
        }, GetResultFromError);
    }

    public static IResult ToOkPasswordUpdateResult(this Result<bool> result)
    {
        return result.Match(isUpdated => Results.Ok(), GetResultFromError);
    }

    public static IResult ToOkIsTakenResult(this Result<bool> result)
    {
        return result.Match(isTaken => Results.Ok(new { isTaken }), e =>
        {
            if (e is ValidationException validationException)
            {
                return Results.BadRequest(new ErrorResultModel { Error = validationException.Message });
            }

            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        });
    }

    public static IResult ToCreatedAtRouteResult(this Result<UserModel> result, string routeName)
    {
        return result.Match(model => Results.CreatedAtRoute(routeName, new { id = model.Id }, model),
            GetResultFromError);
    }

    public static IResult ToCreatedAtRouteResult(this Result<AuthorizedUserModel> result, HttpContext context, string routeName)
    {
        if (result.IsFaulted)
        {
            return GetResultFromError(result.Exception);
        }

        var user = result.Value;
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Expires = new DateTimeOffset(user.RefreshToken.ExpiryDate),
            IsEssential = true,
            Path = "/api/users/authorization"
        };
        context.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken.Token, cookieOptions);
        return Results.CreatedAtRoute(routeName, new { id = user.Id }, user.ToAuthorizedResponseModel());
    }

    public static IResult ToCreatedAtRouteResult<TModel>(this Result<TModel> result, string routeName) where TModel : BaseModel
    {
        return result.Match(model => Results.CreatedAtRoute(routeName, new { id = model.Id }, model),
            GetResultFromError);
    }

    private static IResult GetResultFromError(Exception e)
    {
        if (e is ValidationException validationException)
        {
            return Results.BadRequest(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is NotFoundException)
        {
            return Results.NotFound();
        }

        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
}
