namespace Domain.Errors.Base;

public class NotFoundError : Error
{
    public NotFoundError(List<string> errors) : base(errors)
    {
    }

    public NotFoundError(string error) : base(error)
    {
    }
}
