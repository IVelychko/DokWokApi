namespace Domain.Errors.Base;

public class BadRequestError : Error
{
    public BadRequestError(IDictionary<string, string[]> errors) : base(errors)
    {
    }

    public BadRequestError(string objectName, string error) : base(objectName, error)
    {
    }
}
