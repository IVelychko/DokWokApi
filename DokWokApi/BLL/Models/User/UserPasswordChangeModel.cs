using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserPasswordChangeModel
{
    [Required]
    public string? UserId { get; set; }

    [Required] 
    public string? OldPassword { get; set; }

    [Required]
    public string? NewPassword { get; set; }
}
