namespace Domain.Validation;

public class ValidationResult(bool isValid)
{
    public bool IsValid { get; set; } = isValid;

    public bool IsNotFound { get; set; }

    public List<string> Errors { get; set; } = [];
}
