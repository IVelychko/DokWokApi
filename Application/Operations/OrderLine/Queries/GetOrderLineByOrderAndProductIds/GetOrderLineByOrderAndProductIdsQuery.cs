using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetOrderLineByOrderAndProductIds;

public sealed record GetOrderLineByOrderAndProductIdsQuery(long OrderId, long ProductId) : IQuery<OrderLineModel?>;
