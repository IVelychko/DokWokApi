namespace DokWokApi.Exceptions;

public class OrderException : Exception
{
    public OrderException()
    {
    }

    public OrderException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public OrderException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }
}
