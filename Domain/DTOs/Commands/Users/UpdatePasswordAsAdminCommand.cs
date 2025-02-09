using Domain.Abstractions.Messaging;
using Domain.Shared;

namespace Domain.DTOs.Commands.Users;

public sealed record UpdatePasswordAsAdminCommand(long UserId, string NewPassword) : ICommand<Result<Unit>>;
