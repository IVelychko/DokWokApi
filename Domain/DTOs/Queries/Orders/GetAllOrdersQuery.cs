using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Orders;

namespace Domain.DTOs.Queries.Orders;

public sealed record GetAllOrdersQuery() : IQuery<IEnumerable<OrderResponse>>;
