using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Domain.DTOs.Commands.Users;

public sealed record UpdateUserCommand(
    long Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
) : ICommand<Result<UserResponse>>;
