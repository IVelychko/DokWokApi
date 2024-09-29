using Application.Abstractions.Messaging;

namespace Application.Operations.User.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(long Id) : IQuery<UserResponse?>;
