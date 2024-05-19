using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserLoginModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
