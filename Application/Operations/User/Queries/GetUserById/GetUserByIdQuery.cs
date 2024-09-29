using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetUserById;

public sealed record GetUserByIdQuery(long Id) : IQuery<UserResponse?>;
