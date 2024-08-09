using Domain.Models;

namespace Infrastructure.Validation.Orders.Update;

public sealed record UpdateOrderValidationModel(
    long Id,
    string CustomerName,
    string PhoneNumber,
    string Email,
    string? DeliveryAddress,
    string PaymentType,
    decimal TotalOrderPrice,
    DateTime CreationDate,
    string Status,
    string? UserId,
    long? ShopId,
    List<OrderLineModel> OrderLines
);
