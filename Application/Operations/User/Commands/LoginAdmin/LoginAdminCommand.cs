using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.LoginAdmin;

public sealed record LoginAdminCommand(string UserName, string Password) : ICommand<Result<AuthorizedUserResponse>>;
