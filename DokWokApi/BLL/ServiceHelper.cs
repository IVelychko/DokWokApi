using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DokWokApi.BLL;

public static class ServiceHelper
{
    public static T ThrowArgumentNullExceptionIfNull<T>(T? model, string errorMessage)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), errorMessage);
        }

        return model;
    }

    public static void ThrowOrderExceptionIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new OrderException(errorMessage);
        }
    }

    public static void ThrowUserExceptionIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new UserException(errorMessage);
        }
    }

    public static void ThrowUserExceptionIfNotSucceeded(bool succeeded, IEnumerable<IdentityError> errors)
    {
        if (!succeeded)
        {
            var error = errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}");

            throw new UserException(error);
        }
    }

    public static void ThrowUserExceptionIfUserIsNotValid(bool isValid, string errorMessage)
    {
        if (!isValid)
        {
            throw new UserException(errorMessage);
        }
    }
}
