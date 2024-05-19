using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserPutModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Password { get; set; }
}
