namespace Domain.Validation.Users.RefreshToken;

public sealed record RefreshTokenValidationModel(Entities.RefreshToken RefreshToken, string JwtId);
