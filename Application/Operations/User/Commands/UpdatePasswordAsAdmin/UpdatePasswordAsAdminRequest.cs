namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed record UpdatePasswordAsAdminRequest(long UserId, string NewPassword);
