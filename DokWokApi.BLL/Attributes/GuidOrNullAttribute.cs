using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DokWokApi.BLL.Attributes;

public partial class GuidOrNullAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string stringValue)
        {
            return new ValidationResult("The field must be a string.");
        }

        if (GuidRegex().IsMatch(stringValue))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The field is not a valid GUID.");
    }

    [GeneratedRegex(RegularExpressions.Guid)]
    private static partial Regex GuidRegex();
}
