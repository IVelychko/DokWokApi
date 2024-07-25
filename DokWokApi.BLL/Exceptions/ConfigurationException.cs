namespace DokWokApi.BLL.Exceptions;

public class ConfigurationException : Exception
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public ConfigurationException(string message)
        : base(message)
    {
    }

    public ConfigurationException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public ConfigurationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
