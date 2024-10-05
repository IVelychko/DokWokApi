using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<AddTakeawayOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(AddTakeawayOrderCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await orderService.AddAsync(model);
        return result.Match(o => o.ToResponse(), Result<OrderResponse>.Failure);
    }
}
