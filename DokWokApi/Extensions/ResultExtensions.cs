using DokWokApi.BLL.Models;
using DokWokApi.BLL.Models.Order;
using DokWokApi.BLL.Models.ShoppingCart;
using DokWokApi.BLL.Models.User;
using DokWokApi.Exceptions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace DokWokApi.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToOkCart(this Result<Cart?> result, ILogger logger)
    {
        return result.Match(cart =>
        {
            if (cart is null)
            {
                logger.LogError("Cart error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult(cart);
        }, e => GetResultFromErrorCart(e, logger));
    }

    public static IActionResult ToOkPasswordUpdate(this Result<bool> result, ILogger logger)
    {
        return result.Match(isUpdated => new OkResult(), 
            e => GetResultFromError(e, logger));
    }

    public static IActionResult ToOkIsTaken(this Result<bool> result, ILogger logger)
    {
        return result.Match<IActionResult>(isTaken => new OkObjectResult(new { isTaken }), e =>
        {
            if (e is ValidationException validationException)
            {
                logger.LogInformation(validationException, "Validation problem");
                return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
            }

            logger.LogError(e, "Server error");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        });
    }

    public static IActionResult ToOk<TModel>(this Result<TModel> result, ILogger logger)
    {
        return result.Match(model => new OkObjectResult(model), 
            e => GetResultFromError(e, logger));
    }

    public static IActionResult ToCreatedAtActionOrder(this Result<OrderModel> result, ILogger logger, string actionName, string controllerName)
    {
        return result.Match(order => new CreatedAtActionResult(actionName, controllerName, new { id = order.Id }, order), 
            e => GetResultFromErrorOrder(e, logger));
    }

    public static IActionResult ToCreatedAtActionUser(this Result<UserModel> result, ILogger logger, string actionName, string controllerName)
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model), 
            e => GetResultFromError(e, logger));
    }

    public static IActionResult ToCreatedAtAction<TModel>(this Result<TModel> result, ILogger logger, string actionName, string controllerName) where TModel : BaseModel
    {
        return result.Match(model => new CreatedAtActionResult(actionName, controllerName, new { id = model.Id }, model), 
            e => GetResultFromError(e, logger));
    }

    private static IActionResult GetResultFromError(Exception e, ILogger logger)
    {
        if (e is ValidationException validationException)
        {
            logger.LogInformation(validationException, "Validation problem");
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is EntityNotFoundException notFoundException)
        {
            logger.LogInformation(notFoundException, "Not found");
            return new NotFoundResult();
        }
        else if (e is DbException dbException)
        {
            logger.LogError(dbException, "Database error");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        logger.LogError(e, "Server error");
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult GetResultFromErrorOrder(Exception e, ILogger logger)
    {
        if (e is ValidationException validationException)
        {
            logger.LogInformation(validationException, "Validation problem");
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is EntityNotFoundException notFoundException)
        {
            logger.LogInformation(notFoundException, "Not found");
            return new NotFoundResult();
        }
        else if (e is CartException cartException)
        {
            logger.LogError(cartException, "Cart error");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        else if (e is DbException dbException)
        {
            logger.LogError(dbException, "Database error");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        logger.LogError(e, "Server error");
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    private static IActionResult GetResultFromErrorCart(Exception e, ILogger logger)
    {
        if (e is ValidationException validationException)
        {
            logger.LogInformation(validationException, "Validation problem");
            return new BadRequestObjectResult(new ErrorResultModel { Error = validationException.Message });
        }
        else if (e is EntityNotFoundException notFoundException)
        {
            logger.LogInformation(notFoundException, "Not found");
            return new NotFoundResult();
        }
        else if (e is CartNotFoundException cartNotFoundException)
        {
            logger.LogInformation(cartNotFoundException, "Not found");
            return new NotFoundResult();
        }

        logger.LogError(e, "Server error");
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}
