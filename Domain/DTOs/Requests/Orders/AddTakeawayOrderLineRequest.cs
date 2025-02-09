namespace Domain.DTOs.Requests.Orders;

public sealed record AddTakeawayOrderLineRequest(long ProductId, int Quantity);
