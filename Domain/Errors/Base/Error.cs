namespace Domain.Errors.Base;

public class Error
{
    public IDictionary<string, string[]> Errors { get; set; }

    public Error(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }

    public Error(string objectName, string error)
    {
        Errors = new Dictionary<string, string[]>() { [objectName] = [error] };
    }
}
