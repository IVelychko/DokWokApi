namespace Domain.Exceptions.Base;

public class ConfigurationException : BaseException
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public ConfigurationException(string message)
        : base(message)
    {
    }

    public ConfigurationException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public ConfigurationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
