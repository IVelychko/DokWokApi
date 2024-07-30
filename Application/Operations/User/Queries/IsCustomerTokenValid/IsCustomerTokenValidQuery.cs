using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.IsCustomerTokenValid;

public sealed record IsCustomerTokenValidQuery(string Token) : IQuery<UserResponse?>;
