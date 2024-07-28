using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public class AddDeliveryOrderCommandHandler : ICommandHandler<AddDeliveryOrderCommand, Result<OrderModel>>
{
    private readonly IOrderService _orderService;

    public AddDeliveryOrderCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<Result<OrderModel>> Handle(AddDeliveryOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
