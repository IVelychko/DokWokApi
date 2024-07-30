using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;

public class GetAllOrderLinesByOrderIdQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByOrderIdQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var orderLines = await orderLineService.GetAllByOrderIdAsync(request.OrderId);
        return orderLines.Select(ol => ol.ToResponse());
    }
}
