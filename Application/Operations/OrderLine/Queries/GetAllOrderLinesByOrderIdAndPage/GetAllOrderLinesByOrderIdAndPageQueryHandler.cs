using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderIdAndPage;
public sealed class GetAllOrderLinesByOrderIdAndPageQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByOrderIdAndPageQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByOrderIdAndPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var orderLines = await orderLineService.GetAllByOrderIdAsync(request.OrderId, pageInfo);
        return orderLines.Select(ol => ol.ToResponse());
    }
}
