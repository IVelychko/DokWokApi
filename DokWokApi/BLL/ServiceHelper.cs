using DokWokApi.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DokWokApi.BLL;

public static class ServiceHelper
{
    public static T ThrowIfNull<T>(T? model, string errorMessage)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), errorMessage);
        }

        return model;
    }

    public static void ThrowOrderExcepyionIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new OrderException(nameof(value), errorMessage);
        }
    }

    public static void ThrowUserExcepyionIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new UserException(nameof(value), errorMessage);
        }
    }

    public static void ThrowIfNotSucceeded(bool succeeded, IEnumerable<IdentityError> errors)
    {
        if (!succeeded)
        {
            var error = errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}");

            throw new UserException(nameof(succeeded), error);
        }
    }

    public static void ThrowIfUserIsNotValid(bool isValid, string errorMessage)
    {
        if (!isValid)
        {
            throw new UserException(nameof(isValid), errorMessage);
        }
    }
}
