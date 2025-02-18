using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<AddTakeawayOrderCommand,OrderResponse>
{
    public async Task<OrderResponse> Handle(AddTakeawayOrderCommand request, CancellationToken cancellationToken)
    {
        return await orderService.AddAsync(request);
    }
}
