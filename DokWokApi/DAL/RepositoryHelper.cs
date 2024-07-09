using DokWokApi.Exceptions;

namespace DokWokApi.DAL;

public static class RepositoryHelper
{
    public static T ThrowArgumentNullExceptionIfNull<T>(T? entity, string errorMessage) where T : class
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), errorMessage);
        }

        return entity;
    }

    public static T ThrowEntityNotFoundExceptionIfNull<T>(T? entity, string errorMessage) where T : class
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(errorMessage);
        }

        return entity;
    }

    public static void ThrowArgumentExceptionIfTrue(bool value, string errorMessage)
    {
        if (value)
        {
            throw new ArgumentException(errorMessage);
        }
    }
}
