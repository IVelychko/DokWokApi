using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetOrderLineByOrderAndProductIds;

public class GetOrderLineByOrderAndProductIdsQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetOrderLineByOrderAndProductIdsQuery, OrderLineModel?>
{
    public async Task<OrderLineModel?> Handle(GetOrderLineByOrderAndProductIdsQuery request, CancellationToken cancellationToken) =>
        await orderLineService.GetByOrderAndProductIdsAsync(request.OrderId, request.ProductId);
}
