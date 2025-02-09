using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;
using Domain.Shared;

namespace Domain.DTOs.Commands.Users;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<AuthorizedUserResponse>>;
