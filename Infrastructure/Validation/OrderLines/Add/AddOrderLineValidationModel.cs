namespace Infrastructure.Validation.OrderLines.Add;

public sealed record AddOrderLineValidationModel(long OrderId, long ProductId, int Quantity);
