using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.IsAdminTokenValid;

public sealed record IsAdminTokenValidQuery(string Token) : IQuery<UserModel?>;
