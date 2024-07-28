using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.OrderLine.Commands.DeleteOrderLine;

public class DeleteOrderLineCommandHandler(IOrderLineService orderLineService) : ICommandHandler<DeleteOrderLineCommand, bool?>
{
    public async Task<bool?> Handle(DeleteOrderLineCommand request, CancellationToken cancellationToken)
    {
        var result = await orderLineService.DeleteAsync(request.Id);
        return result;
    }
}
