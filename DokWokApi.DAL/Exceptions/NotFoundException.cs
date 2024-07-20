namespace DokWokApi.DAL.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public NotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
