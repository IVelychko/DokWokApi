using Application.Abstractions.Messaging;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    string Id,
    string FirstName,
    string UserName,
    string Email,
    string PhoneNumber
) : ICommand<Result<UserModel>>;
