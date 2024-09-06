using Domain.Entities;
using System.Text.Json.Serialization;

namespace Application.Operations.User;

public sealed class AuthorizedUserResponse : BaseResponse<string>
{
    public required string? FirstName { get; set; }

    public required string? UserName { get; set; }

    public required string? Email { get; set; }

    public required string? PhoneNumber { get; set; }

    public required string Token { get; set; }

    [JsonIgnore]
    public RefreshToken? RefreshToken { get; set; }

    public required IEnumerable<string> Roles { get; set; }
}
