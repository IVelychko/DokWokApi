using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.IsAdminTokenValid;

public sealed record IsAdminTokenValidQuery(string Token) : IQuery<UserResponse?>;
