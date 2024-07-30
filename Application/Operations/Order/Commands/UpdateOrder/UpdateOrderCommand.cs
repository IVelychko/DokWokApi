using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

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
    string? UserId,
    long? ShopId
) : ICommand<Result<OrderModel>>;
