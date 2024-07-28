using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public class AddOrderLineCommandHandler(IOrderLineService orderLineService) : ICommandHandler<AddOrderLineCommand, Result<OrderLineModel>>
{
    public async Task<Result<OrderLineModel>> Handle(AddOrderLineCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderLineService.AddAsync(model);
        return result;
    }
}
