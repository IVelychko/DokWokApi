using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderIdAndPage;

public sealed record GetAllOrderLinesByOrderIdAndPageQuery(long OrderId, int PageNumber, int PageSize)
    : IQuery<IEnumerable<OrderLineResponse>>;
