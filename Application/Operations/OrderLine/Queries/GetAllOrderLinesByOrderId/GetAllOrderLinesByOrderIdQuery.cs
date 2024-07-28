using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;

public sealed record GetAllOrderLinesByOrderIdQuery(long OrderId) : IQuery<IEnumerable<OrderLineModel>>;