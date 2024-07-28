using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.IsCustomerTokenValid;

public sealed record IsCustomerTokenValidQuery(string Token) : IQuery<UserModel?>;
