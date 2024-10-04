using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.OrderLine.Commands.DeleteOrderLine;

public class DeleteOrderLineCommandHandler(IOrderLineService orderLineService) : ICommandHandler<DeleteOrderLineCommand>
{
    public async Task Handle(DeleteOrderLineCommand request, CancellationToken cancellationToken) =>
        await orderLineService.DeleteAsync(request.Id);
}
