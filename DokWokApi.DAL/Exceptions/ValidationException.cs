namespace DokWokApi.DAL.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
    {
    }

    public ValidationException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public ValidationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
