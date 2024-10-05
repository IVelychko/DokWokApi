using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : ICommand<Result<AuthorizedUserResponse>>;
