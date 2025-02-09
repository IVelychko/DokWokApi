using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Shared;

namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public class AddOrderLineCommandHandler(IOrderLineService orderLineService) : ICommandHandler<AddOrderLineCommand, Result<OrderLineResponse>>
{
    public async Task<Result<OrderLineResponse>> Handle(AddOrderLineCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderLineService.AddAsync(model);
        return result.Match(ol => ol.ToResponse(), Result<OrderLineResponse>.Failure);
    }
}
