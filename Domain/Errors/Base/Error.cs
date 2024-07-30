namespace Domain.Errors.Base;

public class Error
{
    public List<string> Errors { get; set; }

    public Error(List<string> errors)
    {
        Errors = errors;
    }

    public Error(string error)
    {
        Errors = [error];
    }
}
