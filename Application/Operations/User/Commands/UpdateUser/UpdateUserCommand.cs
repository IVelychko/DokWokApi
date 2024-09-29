using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    long Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    long UserRoleId
) : ICommand<Result<UserResponse>>;
