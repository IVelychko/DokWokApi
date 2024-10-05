using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public sealed record AddTakeawayOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string PaymentType,
    long? UserId,
    long ShopId,
    List<AddTakeawayOrderLineRequest> OrderLines
) : ICommand<Result<OrderResponse>>;
