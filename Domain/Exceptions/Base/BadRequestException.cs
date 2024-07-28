namespace Domain.Exceptions.Base;

public class BadRequestException : BaseException
{
    public BadRequestException()
    {
    }

    public BadRequestException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public BadRequestException(string message)
        : base(message)
    {
    }

    public BadRequestException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public BadRequestException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
