namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed record AddDeliveryOrderRequest(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string DeliveryAddress,
    string PaymentType,
    string? UserId,
    List<AddDeliveryOrderLineRequest> OrderLines
);
