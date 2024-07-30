using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetUserByUserName;

public sealed record GetUserByUserNameQuery(string UserName) : IQuery<UserResponse?>;
