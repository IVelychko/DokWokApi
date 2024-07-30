namespace Domain.Errors.Base;

public class BadRequestError : Error
{
    public BadRequestError(List<string> errors) : base(errors)
    {
    }

    public BadRequestError(string error) : base(error)
    {
    }
}
