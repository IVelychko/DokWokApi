namespace Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public NotFoundException(string paramName, string message)
    {
        Errors = new Dictionary<string, string[]>()
        {
            [paramName] = [message]
        };
    }
    
    public IDictionary<string, string[]> Errors { get; init; }
}
