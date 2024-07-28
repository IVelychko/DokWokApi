namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public sealed record AddTakeawayOrderRequest(
    string CustomerName,
    string PhoneNumber,
    string Email,
    string PaymentType,
    string? UserId,
    long ShopId
);
