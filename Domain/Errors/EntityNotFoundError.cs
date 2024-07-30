using Domain.Errors.Base;

namespace Domain.Errors;

public class EntityNotFoundError : NotFoundError
{
    public EntityNotFoundError(List<string> errors) : base(errors)
    {
    }

    public EntityNotFoundError(string error) : base(error)
    {
    }
}
