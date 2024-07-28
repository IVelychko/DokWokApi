namespace Domain.Exceptions.Base;

public class NotFoundException : BaseException
{
    public NotFoundException()
    {
    }

    public NotFoundException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public NotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
