using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses;

namespace Domain.DTOs.Queries.Users;

public sealed record IsUserNameTakenQuery(string UserName) : IQuery<IsTakenResponse>;
