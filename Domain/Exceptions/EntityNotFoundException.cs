using Domain.Exceptions.Base;

namespace Domain.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public EntityNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
