using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserRegisterModel
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? UserName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Password { get; set; }
}
