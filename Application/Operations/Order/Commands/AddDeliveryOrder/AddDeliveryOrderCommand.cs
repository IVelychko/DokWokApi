using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed record AddDeliveryOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string DeliveryAddress,
    string PaymentType,
    long? UserId,
    List<AddDeliveryOrderLineRequest> OrderLines
) : ICommand<Result<OrderResponse>>;
