using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetAllCustomers;

public sealed record GetAllCustomersQuery() : IQuery<IEnumerable<UserResponse>>;
