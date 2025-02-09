using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderIdAndPage;
public sealed class GetAllOrderLinesByOrderIdAndPageQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByOrderIdAndPageQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesByOrderIdAndPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var orderLines = await orderLineService.GetAllByOrderIdAsync(request.OrderId, pageInfo);
        return orderLines.Select(ol => ol.ToResponse());
    }
}
