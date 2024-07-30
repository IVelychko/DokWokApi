using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;

public sealed record GetAllOrderLinesByOrderIdQuery(long OrderId) : IQuery<IEnumerable<OrderLineResponse>>;