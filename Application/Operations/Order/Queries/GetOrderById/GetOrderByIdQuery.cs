using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(long Id) : IQuery<OrderModel?>;
