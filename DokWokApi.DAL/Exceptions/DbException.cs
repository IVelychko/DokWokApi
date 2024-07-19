namespace DokWokApi.DAL.Exceptions;

public class DbException : Exception
{
    public DbException()
    {
    }

    public DbException(string paramName, string message)
        : base($"{message}; Parameter name: {paramName}")
    {
    }

    public DbException(string message)
        : base(message)
    {
    }

    public DbException(string paramName, string message, Exception inner)
        : base($"{message}; Parameter name: {paramName}", inner)
    {
    }

    public DbException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
