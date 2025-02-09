namespace Domain.DTOs.Requests.Users;

public sealed record AddUserRequest(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
);
