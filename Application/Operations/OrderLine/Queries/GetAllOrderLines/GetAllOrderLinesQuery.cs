using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLines;

public sealed record GetAllOrderLinesQuery() : IQuery<IEnumerable<OrderLineResponse>>;
