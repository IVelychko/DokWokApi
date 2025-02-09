namespace Domain.DTOs.Requests.Orders;

public sealed record AddDeliveryOrderLineRequest(long ProductId, int Quantity);
