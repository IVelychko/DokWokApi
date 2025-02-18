using Domain.Abstractions.Messaging;
using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.Orders;

namespace Domain.DTOs.Commands.Orders;

public sealed record AddTakeawayOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string PaymentType,
    long? UserId,
    long ShopId,
    List<AddTakeawayOrderLineRequest> OrderLines
) : ICommand<OrderResponse>;
