using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetUserByUserName;

public sealed record GetUserByUserNameQuery(string UserName) : IQuery<UserModel?>;
