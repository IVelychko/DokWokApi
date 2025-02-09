namespace Domain.DTOs.Requests.OrderLines;

public sealed record UpdateOrderLineRequest(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity
);
