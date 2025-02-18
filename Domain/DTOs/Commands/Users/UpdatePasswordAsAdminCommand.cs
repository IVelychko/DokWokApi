using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Users;

public sealed record UpdatePasswordAsAdminCommand(long UserId, string NewPassword) : ICommand;
