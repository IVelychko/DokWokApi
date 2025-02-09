using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Users;

namespace Domain.DTOs.Queries.Users;

public sealed record GetAllUsersByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<UserResponse>>;
