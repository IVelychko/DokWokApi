namespace Application.Operations.User.Commands.UpdatePassword;

public sealed record UpdatePasswordRequest(long UserId, string OldPassword, string NewPassword);
