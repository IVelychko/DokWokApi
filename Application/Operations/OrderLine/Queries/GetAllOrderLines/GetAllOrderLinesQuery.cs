using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLines;

public sealed record GetAllOrderLinesQuery() : IQuery<IEnumerable<OrderLineModel>>;
