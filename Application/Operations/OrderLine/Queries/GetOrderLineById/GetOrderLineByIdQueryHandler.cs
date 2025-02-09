using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Application.Operations.OrderLine.Queries.GetOrderLineById;

public class GetOrderLineByIdQueryHandler(IOrderLineService orderLineService)
    : IQueryHandler<GetOrderLineByIdQuery, OrderLineResponse?>
{
    public async Task<OrderLineResponse?> Handle(GetOrderLineByIdQuery request, CancellationToken cancellationToken)
    {
        var orderLine = await orderLineService.GetByIdAsync(request.Id);
        return orderLine?.ToResponse();
    }
}
