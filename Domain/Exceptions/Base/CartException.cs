namespace Domain.Exceptions.Base;

public class CartException : BaseException
{
    public CartException()
    {
    }

    public CartException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public CartException(string message)
        : base(message)
    {
    }

    public CartException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public CartException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
