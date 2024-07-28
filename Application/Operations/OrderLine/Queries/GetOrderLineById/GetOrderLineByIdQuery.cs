using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetOrderLineById;

public sealed record GetOrderLineByIdQuery(long Id) : IQuery<OrderLineModel?>;
