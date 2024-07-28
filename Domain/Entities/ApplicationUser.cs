using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public ICollection<Order> Orders { get; set; } = [];
}
