namespace Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public ValidationException(string parameterName, string message)
    {
        Errors = new Dictionary<string, string[]>
        {
            [parameterName] = [message],
        };
    }

    public IDictionary<string, string[]> Errors { get; }
}
