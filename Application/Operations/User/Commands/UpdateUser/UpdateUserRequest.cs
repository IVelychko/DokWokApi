namespace Application.Operations.User.Commands.UpdateUser;

public sealed record UpdateUserRequest(
    long Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
);
