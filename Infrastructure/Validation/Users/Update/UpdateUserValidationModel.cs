namespace Infrastructure.Validation.Users.Update;

public sealed record UpdateUserValidationModel(
    string Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
);
