using Application.Abstractions.Messaging;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.AddUser;

public sealed record AddUserCommand(
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password
) : ICommand<Result<UserModel>>;