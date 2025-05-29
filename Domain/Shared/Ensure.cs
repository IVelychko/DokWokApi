using Domain.Exceptions;

namespace Domain.Shared;

public static class Ensure
{
    public static void ArgumentNotNull(object? argument) => ArgumentNullException.ThrowIfNull(argument);

    public static void ArgumentNotNullOrWhiteSpace(string? argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ValidationException(argumentName, "The value is null or whitespace");
        }
    }

    public static T EntityExists<T>(T? entity, string? message = null) =>
        entity ?? throw new EntityNotFoundException(message);
}