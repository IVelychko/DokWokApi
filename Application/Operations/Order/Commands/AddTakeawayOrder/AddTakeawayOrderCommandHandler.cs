using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<AddTakeawayOrderCommand, Result<OrderModel>>
{
    public async Task<Result<OrderModel>> Handle(AddTakeawayOrderCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderService.AddAsync(model);
        return result;
    }
}
