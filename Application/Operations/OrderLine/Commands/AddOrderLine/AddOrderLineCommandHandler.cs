using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public class AddOrderLineCommandHandler(IOrderLineService orderLineService) 
    : ICommandHandler<AddOrderLineCommand, OrderLineResponse>
{
    public async Task<OrderLineResponse> Handle(AddOrderLineCommand request, CancellationToken cancellationToken)
    {
        return await orderLineService.AddAsync(request);
    }
}
