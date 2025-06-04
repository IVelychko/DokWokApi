namespace Domain.DTOs.Requests.Users;

public sealed record LoginUserRequest(string UserName, string Password);