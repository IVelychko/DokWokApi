namespace Domain.Validation;

public class ValidationResult(bool isValid)
{
    public bool IsValid { get; set; } = isValid;

    public List<string> Errors { get; set; } = [];
}
