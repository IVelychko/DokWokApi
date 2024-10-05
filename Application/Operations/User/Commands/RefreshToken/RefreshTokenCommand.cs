using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.User.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<AuthorizedUserResponse>>;
