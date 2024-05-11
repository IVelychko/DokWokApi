using DokWokApi.DAL.Entities;
using DokWokApi.Exceptions;

namespace DokWokApi.DAL;

public static class RepositoryHelper
{
    public static void CheckForNull(BaseEntity? entity, string errorMessage)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), errorMessage);
        }
    }

    public static void CheckRetrievedEntity(BaseEntity? entity, string errorMessage)
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(entity), errorMessage);
        }
    }

    public static void ThrowIfExists(bool exists, string errorMessage)
    {
        if (exists)
        {
            throw new ArgumentException(errorMessage);
        }
    }
}
