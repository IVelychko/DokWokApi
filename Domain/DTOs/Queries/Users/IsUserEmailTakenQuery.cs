using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Domain.DTOs.Queries.Users;

public sealed record IsUserEmailTakenQuery(string Email) : IQuery<Result<IsTakenResponse>>;
