using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.Order.Commands.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken) =>
        await orderService.DeleteAsync(request.Id);
}
