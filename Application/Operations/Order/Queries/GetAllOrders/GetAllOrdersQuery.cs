using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrders;

public sealed record GetAllOrdersQuery() : IQuery<IEnumerable<OrderModel>>;
