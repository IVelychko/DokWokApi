using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.AddUser;

public sealed record AddUserCommand(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : ICommand<Result<UserResponse>>;