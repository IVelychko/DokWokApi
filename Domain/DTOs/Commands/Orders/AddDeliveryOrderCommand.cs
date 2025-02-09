using Domain.Abstractions.Messaging;
using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Shared;

namespace Domain.DTOs.Commands.Orders;

public sealed record AddDeliveryOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string DeliveryAddress,
    string PaymentType,
    long? UserId,
    List<AddDeliveryOrderLineRequest> OrderLines
) : ICommand<Result<OrderResponse>>;
