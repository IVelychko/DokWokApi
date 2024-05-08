namespace DokWokApi.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() 
    {
    }

    public EntityNotFoundException(string paramName, string message) 
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public EntityNotFoundException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }
}
