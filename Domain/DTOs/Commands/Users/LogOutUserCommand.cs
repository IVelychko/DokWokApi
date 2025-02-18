using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Users;

public sealed record LogOutUserCommand(string RefreshToken) : ICommand;
