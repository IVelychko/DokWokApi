using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;

namespace Application.Operations.Order.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<UpdateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        return await orderService.UpdateAsync(request);
    }
}
