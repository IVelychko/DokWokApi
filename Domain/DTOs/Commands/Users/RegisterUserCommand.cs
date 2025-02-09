using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Domain.DTOs.Commands.Users;

public sealed record RegisterUserCommand(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : ICommand<Result<AuthorizedUserResponse>>;
