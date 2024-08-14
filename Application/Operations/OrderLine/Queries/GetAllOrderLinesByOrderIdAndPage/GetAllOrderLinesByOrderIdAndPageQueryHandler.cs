using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderIdAndPage;
public sealed class GetAllOrderLinesByOrderIdAndPageQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByOrderIdAndPageQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByOrderIdAndPageQuery request, CancellationToken cancellationToken)
    {
        var orderLines = await orderLineService.GetAllByOrderIdAndPageAsync(request.OrderId, request.PageNumber, request.PageSize);
        return orderLines.Select(ol => ol.ToResponse());
    }
}
