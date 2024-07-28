using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLines;

public class GetAllOrderLinesQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesQuery, IEnumerable<OrderLineModel>>
{
    public async Task<IEnumerable<OrderLineModel>> Handle(GetAllOrderLinesQuery request, CancellationToken cancellationToken) =>
        await orderLineService.GetAllAsync();
}
