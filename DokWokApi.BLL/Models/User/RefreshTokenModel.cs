using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.User;

public class RefreshTokenModel
{
    [Required]
    public string? Token { get; set; }

    [Required]
    public string? RefreshToken { get; set; }
}
