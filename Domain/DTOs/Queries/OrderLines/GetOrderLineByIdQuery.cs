using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.OrderLines;

namespace Domain.DTOs.Queries.OrderLines;

public sealed record GetOrderLineByIdQuery(long Id) : IQuery<OrderLineResponse?>;
