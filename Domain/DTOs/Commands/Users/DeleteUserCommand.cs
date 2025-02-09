using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Users;

public sealed record DeleteUserCommand(long Id) : ICommand;
