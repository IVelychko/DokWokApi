using Domain.Entities;
using System.Text.Json.Serialization;

namespace Domain.DTOs.Responses.Users;

public sealed class AuthorizedUserResponse : BaseResponse
{
    public required string FirstName { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Token { get; set; }

    public required long UserRoleId { get; set; }

    public required string UserRole { get; set; }

    [JsonIgnore]
    public RefreshToken? RefreshToken { get; set; }
}
