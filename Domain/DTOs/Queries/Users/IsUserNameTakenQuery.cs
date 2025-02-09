using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Domain.DTOs.Queries.Users;

public sealed record IsUserNameTakenQuery(string UserName) : IQuery<Result<IsTakenResponse>>;
