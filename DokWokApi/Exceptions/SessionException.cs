namespace DokWokApi.Exceptions;

public class SessionException : Exception
{
    public SessionException()
    {
    }

    public SessionException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public SessionException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }
}
