using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserId;

public sealed record GetAllOrdersByUserIdQuery(string UserId) : IQuery<IEnumerable<OrderModel>>;
