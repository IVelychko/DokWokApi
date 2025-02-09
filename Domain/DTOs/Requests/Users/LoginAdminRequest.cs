namespace Domain.DTOs.Requests.Users;

public sealed record LoginAdminRequest(string UserName, string Password);
