using Application.Abstractions.Messaging;
using Domain.Helpers;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePasswordAsAdmin;

public sealed record UpdatePasswordAsAdminCommand(long UserId, string NewPassword) : ICommand<Result<Unit>>;
