using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public class UpdateOrderLineCommandHandler(IOrderLineService orderLineService)
    : ICommandHandler<UpdateOrderLineCommand, OrderLineResponse>
{
    public async Task<OrderLineResponse> Handle(UpdateOrderLineCommand request, CancellationToken cancellationToken)
    {
        return await orderLineService.UpdateAsync(request);
    }
}
