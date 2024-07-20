using DokWokApi.BLL.Models;
using DokWokApi.Models.ShoppingCart;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToOkCartResult(this Result<Cart?> result)
    {
        return result.Match(cart =>
        {
            return cart is not null ? new OkObjectResult(cart) 
                : new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }, GetResultFromError);
    }

    public static IActionResult ToOkPasswordUpdateResult(this Result<bool> result)
    {
        return result.Match(isUpdated => new OkResult(), GetResultFromError);
    }

    public static IActionResult ToOkIsTakenResult(this Result<bool> result)
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

    public static IActionResult ToOkResult<TModel>(this Result<TModel> result)
    {
        return result.Match(model => new OkObjectResult(model), GetResultFromError);
    }

    public static IActionResult ToCreatedAtActionUserResult(this Result<UserModel> result, string actionName, string controllerName)
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model), 
            GetResultFromError);
    }

    public static IActionResult ToCreatedAtActionAuthorizedUserResult(this Result<AuthorizedUserModel> result, string actionName, string controllerName)
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model),
            GetResultFromError);
    }

    public static IActionResult ToCreatedAtActionResult<TModel>(this Result<TModel> result, string actionName, string controllerName) where TModel : BaseModel
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model),
            GetResultFromError);
    }

    private static IActionResult GetResultFromError(Exception e)
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
}
