using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetUserFromToken;

public sealed record GetUserFromTokenQuery(string Token) : IQuery<UserResponse?>;
