namespace Domain.DTOs.Requests.Users;

public sealed record UpdatePasswordAsAdminRequest(long UserId, string NewPassword);
