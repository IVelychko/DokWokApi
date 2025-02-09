using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Orders;

namespace Domain.DTOs.Queries.Orders;

public sealed record GetAllOrdersByUserIdQuery(long UserId) : IQuery<IEnumerable<OrderResponse>>;
