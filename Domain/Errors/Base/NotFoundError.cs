namespace Domain.Errors.Base;

public class NotFoundError : Error
{
    public NotFoundError(IDictionary<string, string[]> errors) : base(errors)
    {
    }

    public NotFoundError(string objectError, string error) : base(objectError, error)
    {
    }
}
