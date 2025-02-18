namespace Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string paramName, string errorMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            [paramName] = [errorMessage]
        };
    }
    
    public ValidationException(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; init; }
}
