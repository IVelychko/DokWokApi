namespace Application.Operations.User.Commands.RegisterUser;

public sealed record RegisterUserRequest(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
);
