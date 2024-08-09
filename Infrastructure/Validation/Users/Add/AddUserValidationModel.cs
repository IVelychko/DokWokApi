namespace Infrastructure.Validation.Users.Add;

public sealed record AddUserValidationModel(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
);
