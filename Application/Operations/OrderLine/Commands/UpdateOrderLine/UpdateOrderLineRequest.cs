namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public sealed record UpdateOrderLineRequest(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity
);
