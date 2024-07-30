using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserId;

public sealed record GetAllOrdersByUserIdQuery(string UserId) : IQuery<IEnumerable<OrderResponse>>;
