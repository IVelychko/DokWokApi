using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetUserById;

public sealed record GetUserByIdQuery(string Id) : IQuery<UserModel?>;
