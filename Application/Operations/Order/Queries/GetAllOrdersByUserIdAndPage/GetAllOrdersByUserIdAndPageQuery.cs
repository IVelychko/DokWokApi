using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Queries.GetAllOrdersByUserIdAndPage;

public sealed record GetAllOrdersByUserIdAndPageQuery(string UserId, int PageNumber, int PageSize) : IQuery<IEnumerable<OrderResponse>>;
