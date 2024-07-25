using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class SecurityTokenModel
{
    [Required]
    public string? Token { get; set; }
}
