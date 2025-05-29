namespace Domain.DTOs.Requests.Users;

public record RefreshUserTokenRequest(string Token, string RefreshToken);