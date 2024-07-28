using Application.Abstractions.Messaging;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.LoginAdmin;

public sealed record LoginAdminCommand(string UserName, string Password) : ICommand<Result<AuthorizedUserModel>>;
