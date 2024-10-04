using Application.Abstractions.Messaging;
using Domain.Helpers;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePassword;

public sealed record UpdatePasswordCommand(long UserId, string OldPassword, string NewPassword) : ICommand<Result<Unit>>;
