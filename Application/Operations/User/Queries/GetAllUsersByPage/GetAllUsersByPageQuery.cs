using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetAllUsersByPage;

public sealed record GetAllUsersByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<UserResponse>>;
