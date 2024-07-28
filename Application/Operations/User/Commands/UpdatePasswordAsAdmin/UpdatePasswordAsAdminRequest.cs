namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed record UpdatePasswordAsAdminRequest(string UserId, string NewPassword);
