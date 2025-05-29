using Application.Constants;
using Domain.Exceptions;
using FluentValidation.Results;

namespace Application.Extensions;

public static class ValidationResultExtensions
{
    public static void ThrowIfValidationFailed(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return;
        }

        var notFoundErrorCodeExists = validationResult.Errors
            .Exists(x => x.ErrorCode == ErrorCodes.EntityNotFound);
        if (notFoundErrorCodeExists)
        {
            throw new EntityNotFoundException("The entity was not found");
        }

        throw new ValidationException(validationResult.ToDictionary());
    }
}