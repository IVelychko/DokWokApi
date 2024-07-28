namespace Domain.Exceptions.Base;

public class BaseException : Exception
{
    public BaseException()
    {
    }

    public BaseException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public BaseException(string message)
        : base(message)
    {
    }

    public BaseException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public BaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
