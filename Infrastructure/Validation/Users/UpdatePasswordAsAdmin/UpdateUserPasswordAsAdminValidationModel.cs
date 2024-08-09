namespace Infrastructure.Validation.Users.UpdatePasswordAsAdmin;

public sealed record UpdateUserPasswordAsAdminValidationModel(string UserId, string NewPassword);
