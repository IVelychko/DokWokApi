using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Commands.UpdatePassword;

public sealed record UpdatePasswordCommand(string UserId, string OldPassword, string NewPassword) : ICommand<Result<bool>>;
