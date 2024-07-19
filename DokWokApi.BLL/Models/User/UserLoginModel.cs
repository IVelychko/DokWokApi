using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserLoginModel
{
    [Required]
    [RegularExpression(RegularExpressions.UserName)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(RegularExpressions.Password)]
    public string Password { get; set; } = string.Empty;
}
