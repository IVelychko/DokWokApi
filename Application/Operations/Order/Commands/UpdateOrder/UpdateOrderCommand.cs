using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.Order.Commands.UpdateOrder;

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
