using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Orders;
using Domain.Shared;

namespace Domain.DTOs.Commands.Orders;

public sealed record UpdateOrderCommand(
    long Id,
    string CustomerName,
    string PhoneNumber,
    string Email,
    string? DeliveryAddress,
    string PaymentType,
    decimal TotalOrderPrice,
    DateTime CreationDate,
    string Status,
    long? UserId,
    long? ShopId
) : ICommand<Result<OrderResponse>>;
