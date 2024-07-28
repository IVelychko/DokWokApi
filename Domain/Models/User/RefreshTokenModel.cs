namespace Domain.Models.User;

public class RefreshTokenModel
{
    public required string Token { get; set; }

    public required string RefreshToken { get; set; }
}
