using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserRegisterModel
{
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

    [Required]
    [RegularExpression(RegularExpressions.Password)]
    public string? Password { get; set; }
}
