using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed record UpdatePasswordAsAdminCommand(string UserId, string NewPassword) : ICommand<Result<bool>>;
