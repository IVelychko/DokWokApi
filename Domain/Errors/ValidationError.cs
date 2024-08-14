using Domain.Errors.Base;

namespace Domain.Errors;

public class ValidationError : BadRequestError
{
    public ValidationError(IDictionary<string, string[]> errors) : base(errors)
    {
    }

    public ValidationError(string objectError, string error) : base(objectError, error)
    {
    }
}
