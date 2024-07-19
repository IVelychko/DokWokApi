using DokWokApi.DAL.Exceptions;
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

    public static void ThrowValidationExceptionIfNotSucceeded(bool succeeded, IEnumerable<IdentityError> errors)
    {
        if (!succeeded)
        {
            var error = errors.Select(e => e.Description)
                .Aggregate((e1, e2) => $"{e1}\n{e2}");

            throw new ValidationException(error);
        }
    }

    public static void ThrowValidationExceptionIfUserIsNotValid(bool isValid, string errorMessage)
    {
        if (!isValid)
        {
            throw new ValidationException(errorMessage);
        }
    }
}
