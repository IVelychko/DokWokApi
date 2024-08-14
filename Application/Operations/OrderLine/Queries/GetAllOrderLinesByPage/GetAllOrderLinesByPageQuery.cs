using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByPage;

public sealed record GetAllOrderLinesByPageQuery(int PageNumber, int PageSize)
    : IQuery<IEnumerable<OrderLineResponse>>;
