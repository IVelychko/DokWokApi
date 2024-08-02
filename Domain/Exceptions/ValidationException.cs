namespace Domain.Exceptions;

public class ValidationException(IDictionary<string, string[]> errors) : Exception
{
    public IDictionary<string, string[]> Errors { get; set; } = errors;
}
