using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetOrderLineById;

public class GetOrderLineByIdQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetOrderLineByIdQuery, OrderLineModel?>
{
    public async Task<OrderLineModel?> Handle(GetOrderLineByIdQuery request, CancellationToken cancellationToken) =>
        await orderLineService.GetByIdAsync(request.Id);
}
