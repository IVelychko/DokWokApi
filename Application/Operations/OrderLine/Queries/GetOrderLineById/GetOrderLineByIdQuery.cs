using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Queries.GetOrderLineById;

public sealed record GetOrderLineByIdQuery(long Id) : IQuery<OrderLineResponse?>;
