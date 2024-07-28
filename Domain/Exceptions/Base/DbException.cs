namespace Domain.Exceptions.Base;

public class DbException : BaseException
{
    public DbException()
    {
    }

    public DbException(string paramName, string message)
        : base(paramName, message)
    {
    }

    public DbException(string message)
        : base(message)
    {
    }

    public DbException(string paramName, string message, Exception inner)
        : base(paramName, message, inner)
    {
    }

    public DbException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
