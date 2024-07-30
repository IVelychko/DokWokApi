using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.Order.Commands.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderService orderService)
    : ICommandHandler<DeleteOrderCommand, bool?>
{
    public async Task<bool?> Handle(DeleteOrderCommand request, CancellationToken cancellationToken) =>
        await orderService.DeleteAsync(request.Id);
}
