namespace Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public long UserRoleId { get; set; }

    public UserRole? UserRole { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public ICollection<Order> Orders { get; set; } = [];
}
