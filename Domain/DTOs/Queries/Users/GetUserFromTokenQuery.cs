using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;

namespace Domain.DTOs.Queries.Users;

public sealed record GetUserFromTokenQuery(string Token) : IQuery<UserResponse?>;
