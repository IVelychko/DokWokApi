using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Shared;

public static class Ensure
{
    public static void ArgumentNotNull<T>(T? argument) => ArgumentNullException.ThrowIfNull(argument);
    
    public static void ArgumentNotNullOrWhiteSpace(string? argument) 
        => ArgumentException.ThrowIfNullOrWhiteSpace(argument);

    public static T EntityFound<T>(T? entity, string? message = null) where T : BaseEntity =>
        entity ?? throw new EntityNotFoundException(message);
}