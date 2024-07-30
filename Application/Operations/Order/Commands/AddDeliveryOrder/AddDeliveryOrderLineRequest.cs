namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed record AddDeliveryOrderLineRequest(long ProductId, int Quantity);
