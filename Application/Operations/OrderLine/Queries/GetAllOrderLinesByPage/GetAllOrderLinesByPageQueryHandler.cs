using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByPage;

public sealed class GetAllOrderLinesByPageQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByPageQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var orderLines = await orderLineService.GetAllAsync(pageInfo);
        return orderLines.Select(ol => ol.ToResponse());
    }
}
