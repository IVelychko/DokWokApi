namespace Domain.DTOs.Requests.Users;

public sealed record RegisterUserRequest(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
);
