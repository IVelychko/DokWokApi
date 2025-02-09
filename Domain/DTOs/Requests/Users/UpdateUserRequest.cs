namespace Domain.DTOs.Requests.Users;

public sealed record UpdateUserRequest(
    long Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
);
