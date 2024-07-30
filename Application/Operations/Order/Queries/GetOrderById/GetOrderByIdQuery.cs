using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(long Id) : IQuery<OrderResponse?>;
