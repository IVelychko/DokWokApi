using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;

namespace Domain.DTOs.Commands.Users;

public sealed record UpdateUserCommand(
    long Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
) : ICommand<UserResponse>;
