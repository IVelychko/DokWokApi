namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public sealed record AddOrderLineRequest(
    long OrderId,
    long ProductId,
    int Quantity
);
