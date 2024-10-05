using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.UpdatePassword;

public sealed record UpdatePasswordCommand(long UserId, string OldPassword, string NewPassword) : ICommand<Result<Unit>>;
