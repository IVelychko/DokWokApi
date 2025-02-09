using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Orders;

namespace Domain.DTOs.Queries.Orders;

public sealed record GetAllOrdersByUserIdAndPageQuery(long UserId, int PageNumber, int PageSize) : IQuery<IEnumerable<OrderResponse>>;
