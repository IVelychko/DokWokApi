using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;

namespace Domain.DTOs.Commands.Users;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<AuthorizedUserResponse>;
