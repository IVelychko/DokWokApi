namespace Application.Operations.User.Commands.AddUser;

public sealed record AddUserRequest(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
);
