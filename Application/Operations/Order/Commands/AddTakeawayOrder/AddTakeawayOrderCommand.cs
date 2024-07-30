using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public sealed record AddTakeawayOrderCommand(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string PaymentType,
    string? UserId,
    long ShopId,
    List<AddTakeawayOrderLineRequest> OrderLines
) : ICommand<Result<OrderModel>>;
