namespace Domain.Models.User;

public class UserModel
{
    public string Id { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }
}
