using Domain.Errors.Base;

namespace Domain.Errors;

public class EntityNotFoundError : NotFoundError
{
    public EntityNotFoundError(IDictionary<string, string[]> errors) : base(errors)
    {
    }

    public EntityNotFoundError(string objectError, string error) : base(objectError, error)
    {
    }
}
