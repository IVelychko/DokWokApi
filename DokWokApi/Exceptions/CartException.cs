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

    public CartException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }
}
