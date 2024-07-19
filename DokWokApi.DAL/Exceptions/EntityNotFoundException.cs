namespace DokWokApi.DAL.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public EntityNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
