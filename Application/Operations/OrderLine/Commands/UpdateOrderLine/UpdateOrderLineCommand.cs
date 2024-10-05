using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public sealed record UpdateOrderLineCommand(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity
) : ICommand<Result<OrderLineResponse>>;
