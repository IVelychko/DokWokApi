namespace Domain.Errors.Base;

public class DbError : Error
{
    public DbError(List<string> errors) : base(errors)
    {
    }

    public DbError(string error) : base(error)
    {
    }
}
