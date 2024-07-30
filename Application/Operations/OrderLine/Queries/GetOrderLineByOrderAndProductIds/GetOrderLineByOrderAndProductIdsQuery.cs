using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Queries.GetOrderLineByOrderAndProductIds;

public sealed record GetOrderLineByOrderAndProductIdsQuery(long OrderId, long ProductId) : IQuery<OrderLineResponse?>;
