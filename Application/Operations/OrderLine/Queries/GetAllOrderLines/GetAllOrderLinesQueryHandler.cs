using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Application.Operations.OrderLine.Queries.GetAllOrderLines;

public class GetAllOrderLinesQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetAllOrderLinesQuery, IEnumerable<OrderLineResponse>>
{
    public async Task<IEnumerable<OrderLineResponse>> Handle(GetAllOrderLinesQuery request, CancellationToken cancellationToken)
    {
        return await orderLineService.GetAllAsync();
    }
}
