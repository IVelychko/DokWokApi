using Application.Abstractions.Messaging;
using Domain.Models.User;
using Domain.ResultType;

namespace Application.Operations.User.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : ICommand<Result<AuthorizedUserModel>>;
