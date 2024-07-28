using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.User.Queries.GetUserRoles;

public sealed record GetUserRolesQuery(string UserId) : IQuery<Result<IEnumerable<string>>>;
