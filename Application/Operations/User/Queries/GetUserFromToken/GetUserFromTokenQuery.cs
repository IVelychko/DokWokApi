using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetUserFromToken;

public sealed record GetUserFromTokenQuery(string Token) : IQuery<UserModel?>;
