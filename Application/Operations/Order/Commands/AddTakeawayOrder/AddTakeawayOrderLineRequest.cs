namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public sealed record AddTakeawayOrderLineRequest(long ProductId, int Quantity);
