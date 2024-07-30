using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetAllUsers;

public sealed record GetAllUsersQuery() : IQuery<IEnumerable<UserResponse>>;
