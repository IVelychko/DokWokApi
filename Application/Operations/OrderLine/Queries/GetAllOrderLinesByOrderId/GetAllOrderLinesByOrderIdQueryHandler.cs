using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLinesByOrderId;

public class GetAllOrderLinesByOrderIdQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesByOrderIdQuery, IEnumerable<OrderLineModel>>
{
    public async Task<IEnumerable<OrderLineModel>> Handle(GetAllOrderLinesByOrderIdQuery request, CancellationToken cancellationToken) =>
        await orderLineService.GetAllByOrderIdAsync(request.OrderId);
}
