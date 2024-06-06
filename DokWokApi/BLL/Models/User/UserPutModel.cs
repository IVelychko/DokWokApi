using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class UserPutModel
{
    [Required]
    public string? Id { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? UserName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }
}
