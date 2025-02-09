using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Orders;

namespace Domain.DTOs.Queries.Orders;

public sealed record GetOrderByIdQuery(long Id) : IQuery<OrderResponse?>;
