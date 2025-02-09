using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Application.Operations.OrderLine.Queries.GetOrderLineByOrderAndProductIds;

public class GetOrderLineByOrderAndProductIdsQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetOrderLineByOrderAndProductIdsQuery, OrderLineResponse?>
{
    public async Task<OrderLineResponse?> Handle(GetOrderLineByOrderAndProductIdsQuery request, CancellationToken cancellationToken)
    {
        var orderLine = await orderLineService.GetByOrderAndProductIdsAsync(request.OrderId, request.ProductId);
        return orderLine?.ToResponse();
    }
}
