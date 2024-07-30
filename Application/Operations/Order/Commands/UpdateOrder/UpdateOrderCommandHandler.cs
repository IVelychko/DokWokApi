using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Order.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<UpdateOrderCommand, Result<OrderModel>>
{
    public async Task<Result<OrderModel>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderService.UpdateAsync(model);
        return result;
    }
}
