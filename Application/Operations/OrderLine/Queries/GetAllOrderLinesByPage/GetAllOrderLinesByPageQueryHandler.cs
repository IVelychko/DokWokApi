using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByPage;

public sealed class GetAllOrderLinesByPageQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByPageQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByPageQuery request, CancellationToken cancellationToken)
    {
        var orderLines = await orderLineService.GetAllByPageAsync(request.PageNumber, request.PageSize);
        return orderLines.Select(ol => ol.ToResponse());
    }
}
