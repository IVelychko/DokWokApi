namespace Domain.DTOs.Requests.Users;

public sealed record UpdatePasswordRequest(long UserId, string OldPassword, string NewPassword);
