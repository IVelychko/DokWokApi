using Domain.Errors.Base;

namespace Domain.Errors;

public class ValidationError : BadRequestError
{
    public ValidationError(List<string> errors) : base(errors)
    {
    }

    public ValidationError(string error) : base(error)
    {
    }
}
