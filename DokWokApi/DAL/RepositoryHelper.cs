using DokWokApi.Exceptions;

namespace DokWokApi.DAL;

public static class RepositoryHelper
{
    public static T CheckForNull<T>(T? entity, string errorMessage) where T : class
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), errorMessage);
        }

        return entity;
    }

    public static T CheckRetrievedEntity<T>(T? entity, string errorMessage) where T : class
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(entity), errorMessage);
        }

        return entity;
    }

    public static void ThrowIfExists(bool exists, string errorMessage)
    {
        if (exists)
        {
            throw new ArgumentException(errorMessage);
        }
    }
}
