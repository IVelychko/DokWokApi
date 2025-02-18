using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;

namespace Domain.DTOs.Commands.Users;

public sealed record AddUserCommand(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : ICommand<UserResponse>;