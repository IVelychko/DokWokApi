using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Users;

public sealed record UpdatePasswordCommand(long UserId, string OldPassword, string NewPassword) 
    : ICommand;
