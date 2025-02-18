using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public class AddDeliveryOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<AddDeliveryOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(AddDeliveryOrderCommand request, CancellationToken cancellationToken)
    {
        return await orderService.AddAsync(request);
    }
}
