using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserId;

public sealed record GetAllOrdersByUserIdQuery(long UserId) : IQuery<IEnumerable<OrderResponse>>;
