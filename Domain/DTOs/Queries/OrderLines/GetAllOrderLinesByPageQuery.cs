using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.OrderLines;

namespace Domain.DTOs.Queries.OrderLines;

public sealed record GetAllOrderLinesByPageQuery(int PageNumber, int PageSize)
    : IQuery<IEnumerable<OrderLineResponse>>;
