using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserPutModel
{
    [Required]
    [RegularExpression(RegularExpressions.Guid)]
    public string? Id { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.FirstName)]
    public string? FirstName { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.UserName)]
    public string? UserName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.PhoneNumber)]
    public string? PhoneNumber { get; set; }
}
