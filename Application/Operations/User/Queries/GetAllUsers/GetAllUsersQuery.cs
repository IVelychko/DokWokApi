using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetAllUsers;

public sealed record GetAllUsersQuery() : IQuery<IEnumerable<UserModel>>;
