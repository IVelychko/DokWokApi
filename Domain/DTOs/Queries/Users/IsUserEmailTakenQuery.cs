using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;

namespace Domain.DTOs.Queries.Users;

public sealed record IsUserEmailTakenQuery(string Email) : IQuery<IsTakenResponse>;
