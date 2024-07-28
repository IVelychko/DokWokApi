using Application.Abstractions.Messaging;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : ICommand<Result<AuthorizedUserModel>>;
