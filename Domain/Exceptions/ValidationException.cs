using Domain.Exceptions.Base;

namespace Domain.Exceptions;

public class ValidationException : BadRequestException
{
    public ValidationException()
    {
    }

    public ValidationException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public ValidationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
