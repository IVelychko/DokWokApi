using Domain.Entities;

namespace Domain.Models.User;

public class AuthorizedUserModel : BaseModel
{
    public string FirstName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public long UserRoleId { get; set; }

    public string UserRole { get; set; } = string.Empty;

    public required string Token { get; set; }

    public required RefreshToken RefreshToken { get; set; }
}
