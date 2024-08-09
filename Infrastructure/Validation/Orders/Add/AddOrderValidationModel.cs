using Domain.Models;

namespace Infrastructure.Validation.Orders.Add;

public sealed record AddOrderValidationModel(
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
