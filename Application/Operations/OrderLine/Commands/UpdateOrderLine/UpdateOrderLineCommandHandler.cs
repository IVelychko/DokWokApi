using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Shared;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public class UpdateOrderLineCommandHandler(IOrderLineService orderLineService)
    : ICommandHandler<UpdateOrderLineCommand, Result<OrderLineResponse>>
{
    public async Task<Result<OrderLineResponse>> Handle(UpdateOrderLineCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderLineService.UpdateAsync(model);
        return result.Match(ol => ol.ToResponse(), Result<OrderLineResponse>.Failure);
    }
}
