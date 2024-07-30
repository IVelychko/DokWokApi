using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetUserById;

public sealed record GetUserByIdQuery(string Id) : IQuery<UserResponse?>;
