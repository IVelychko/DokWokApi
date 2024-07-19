using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DokWokApi.BLL.Attributes;

public partial class AddressOrNullAttribute : ValidationAttribute
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

        if (AddressRegex().IsMatch(stringValue))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The field is not a valid address.");
    }

    [GeneratedRegex(RegularExpressions.Address)]
    private static partial Regex AddressRegex();
}
