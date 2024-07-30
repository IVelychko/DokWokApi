using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

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
