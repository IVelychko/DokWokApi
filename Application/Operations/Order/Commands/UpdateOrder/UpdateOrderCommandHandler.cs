using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Shared;

namespace Application.Operations.Order.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<UpdateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderService.UpdateAsync(model);
        return result.Match(o => o.ToResponse(), Result<OrderResponse>.Failure);
    }
}
