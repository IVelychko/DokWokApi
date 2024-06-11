using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserPasswordChangeAsAdminModel
{
    [Required]
    [RegularExpression(RegularExpressions.Guid)]
    public string? UserId { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.Password)]
    public string? NewPassword { get; set; }
}
