using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public class UpdateOrderLineCommandHandler(IOrderLineService orderLineService) : ICommandHandler<UpdateOrderLineCommand, Result<OrderLineModel>>
{
    public async Task<Result<OrderLineModel>> Handle(UpdateOrderLineCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderLineService.UpdateAsync(model);
        return result;
    }
}
