namespace Domain.Exceptions;

public class NotFoundException(IDictionary<string, string[]> errors) : Exception
{
    public IDictionary<string, string[]> Errors { get; set; } = errors;
}
