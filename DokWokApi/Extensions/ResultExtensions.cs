using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.Order;
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
            if (cart is null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult(cart);
        }, e => GetResultFromErrorCart(e));
    }

    public static IActionResult ToOkPasswordUpdateResult(this Result<bool> result)
    {
        return result.Match(isUpdated => new OkResult(), 
            e => GetResultFromError(e));
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
        return result.Match(model => new OkObjectResult(model), 
            e => GetResultFromError(e));
    }

    public static IActionResult ToCreatedAtActionOrderResult(this Result<OrderModel> result, string actionName, string controllerName)
    {
        return result.Match(order => new CreatedAtActionResult(actionName, controllerName, new { id = order.Id }, order), 
            e => GetResultFromErrorOrder(e));
    }

    public static IActionResult ToCreatedAtActionUserResult(this Result<UserModel> result, string actionName, string controllerName)
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model), 
            e => GetResultFromError(e));
    }

    public static IActionResult ToCreatedAtActionResult<TModel>(this Result<TModel> result, string actionName, string controllerName) where TModel : BaseModel
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model), 
            e => GetResultFromError(e));
    }

    private static IActionResult GetResultFromError(Exception e)
    {
        if (e is ValidationException validationException)
        {
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is EntityNotFoundException)
        {
            return new NotFoundResult();
        }
        else if (e is DbException)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult GetResultFromErrorOrder(Exception e)
    {
        if (e is ValidationException validationException)
        {
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is EntityNotFoundException)
        {
            return new NotFoundResult();
        }
        else if (e is CartException)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        else if (e is DbException)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult GetResultFromErrorCart(Exception e)
    {
        if (e is ValidationException validationException)
        {
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is EntityNotFoundException)
        {
            return new NotFoundResult();
        }
        else if (e is CartNotFoundException)
        {
            return new NotFoundResult();
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}
