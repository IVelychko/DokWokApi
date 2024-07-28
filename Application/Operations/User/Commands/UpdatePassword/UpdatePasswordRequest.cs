namespace Application.Operations.User.Commands.UpdatePassword;

public sealed record UpdatePasswordRequest(string UserId, string OldPassword, string NewPassword);
