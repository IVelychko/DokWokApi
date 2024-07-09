namespace DokWokApi.Exceptions;

public class CartNotFoundException : CartException
{
    public CartNotFoundException()
    {
    }

    public CartNotFoundException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public CartNotFoundException(string message)
        : base(message)
    {
    }

    public CartNotFoundException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public CartNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
