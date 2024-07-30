using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<AuthorizedUserResponse>>;
