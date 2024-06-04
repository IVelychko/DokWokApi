using DokWokApi.Exceptions;

namespace DokWokApi.DAL;

public static class RepositoryHelper
{
    public static T ThrowIfNull<T>(T? entity, string errorMessage) where T : class
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), errorMessage);
        }

        return entity;
    }

    public static T ThrowEntityNotFoundIfNull<T>(T? entity, string errorMessage) where T : class
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(entity), errorMessage);
        }

        return entity;
    }

    public static void ThrowIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new ArgumentException(errorMessage, nameof(value));
        }
    }
}
