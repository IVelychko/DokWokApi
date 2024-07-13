namespace DokWokApi.Validation;

public class ValidationResult
{
    public bool IsValid { get; set; }

    public bool IsFound { get; set; }

    public string Error { get; set; } = string.Empty;
}
