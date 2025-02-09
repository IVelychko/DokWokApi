using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.OrderLines;

namespace Domain.DTOs.Queries.OrderLines;

public sealed record GetOrderLineByOrderAndProductIdsQuery(long OrderId, long ProductId) : IQuery<OrderLineResponse?>;
