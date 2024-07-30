using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed record AddDeliveryOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string DeliveryAddress,
    string PaymentType,
    string? UserId,
    List<AddDeliveryOrderLineRequest> OrderLines
) : ICommand<Result<OrderResponse>>;
