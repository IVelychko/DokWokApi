namespace Application.Operations.User;

public class UserResponse
{
    public required string Id { get; set; }

    public required string? FirstName { get; set; }

    public required string? UserName { get; set; }

    public required string? Email { get; set; }

    public required string? PhoneNumber { get; set; }
}
