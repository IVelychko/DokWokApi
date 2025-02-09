namespace Domain.DTOs.Requests.OrderLines;

public sealed record AddOrderLineRequest(
    long OrderId,
    long ProductId,
    int Quantity
);
