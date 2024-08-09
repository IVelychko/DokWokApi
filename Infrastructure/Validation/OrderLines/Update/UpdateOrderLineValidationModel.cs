namespace Infrastructure.Validation.OrderLines.Update;

public sealed record UpdateOrderLineValidationModel(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity
);
