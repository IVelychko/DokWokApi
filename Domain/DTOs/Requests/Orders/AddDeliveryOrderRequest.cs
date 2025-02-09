namespace Domain.DTOs.Requests.Orders;

public sealed record AddDeliveryOrderRequest(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string DeliveryAddress,
    string PaymentType,
    long? UserId,
    List<AddDeliveryOrderLineRequest> OrderLines
);
