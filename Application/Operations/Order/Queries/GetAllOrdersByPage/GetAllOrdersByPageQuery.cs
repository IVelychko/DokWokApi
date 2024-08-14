using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Queries.GetAllOrdersByPage;

public sealed record GetAllOrdersByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<OrderResponse>>;
