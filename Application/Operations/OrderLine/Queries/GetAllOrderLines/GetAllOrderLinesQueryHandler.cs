using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLines;

public class GetAllOrderLinesQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesQuery request, CancellationToken cancellationToken)
    {
        var orderLines = await orderLineService.GetAllAsync();
        return orderLines.Select(ol => ol.ToResponse());
    }
}
