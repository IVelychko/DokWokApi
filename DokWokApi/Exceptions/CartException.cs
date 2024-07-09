namespace DokWokApi.Exceptions;

public class CartException : Exception
{
    public CartException() 
    { 
    }

    public CartException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public CartException(string message)
        : base(message)
    {
    }

    public CartException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public CartException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
