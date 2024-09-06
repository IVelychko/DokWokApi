using Domain.Entities;

namespace Domain.Models.User;

public class AuthorizedUserModel
{
    public string Id { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public required string Token { get; set; }

    public required RefreshToken RefreshToken { get; set; }

    public required IEnumerable<string> Roles { get; set; }
}
