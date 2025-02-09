using Domain.Abstractions.Messaging;
using Domain.Shared;

namespace Domain.DTOs.Commands.Users;

public sealed record UpdatePasswordCommand(long UserId, string OldPassword, string NewPassword) : ICommand<Result<Unit>>;
