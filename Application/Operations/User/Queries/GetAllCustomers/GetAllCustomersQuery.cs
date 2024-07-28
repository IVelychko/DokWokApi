using Application.Abstractions.Messaging;
using Domain.Models.User;

namespace Application.Operations.User.Queries.GetAllCustomers;

public sealed record GetAllCustomersQuery() : IQuery<IEnumerable<UserModel>>;
