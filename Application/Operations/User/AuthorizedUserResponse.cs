namespace Application.Operations.User;

public sealed class AuthorizedUserResponse
{
    public required string Id { get; set; }

    public required string? FirstName { get; set; }

    public required string? UserName { get; set; }

    public required string? Email { get; set; }

    public required string? PhoneNumber { get; set; }

    public required string Token { get; set; }
}
