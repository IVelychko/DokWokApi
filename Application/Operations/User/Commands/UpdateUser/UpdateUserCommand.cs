using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    long Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
) : ICommand<Result<UserResponse>>;
