namespace Infrastructure.Validation.Users.UpdatePassword;

public sealed record UpdateUserPasswordValidationModel(string UserId, string OldPassword, string NewPassword);
