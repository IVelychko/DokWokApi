using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Queries.GetAllOrders;

public sealed record GetAllOrdersQuery() : IQuery<IEnumerable<OrderResponse>>;
