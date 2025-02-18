namespace Domain.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(string paramName, string message) 
        : base(paramName, message)
    {
    }

    public EntityNotFoundException(IDictionary<string, string[]> errors)
        : base(errors)
    {
    }
}