using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.OrderLines;

namespace Application.Operations.OrderLine.Commands.DeleteOrderLine;

public class DeleteOrderLineCommandHandler(IOrderLineService orderLineService) : ICommandHandler<DeleteOrderLineCommand>
{
    public async Task Handle(DeleteOrderLineCommand request, CancellationToken cancellationToken) =>
        await orderLineService.DeleteAsync(request.Id);
}
