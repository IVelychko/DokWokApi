using Domain.DTOs.Responses.OrderLines;

namespace Domain.DTOs.Responses.Orders;

public class OrderResponse : BaseResponse
{
    public required string CustomerName { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Email { get; set; }

    public required string? DeliveryAddress { get; set; }

    public required string PaymentType { get; set; }

    public required decimal TotalOrderPrice { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string Status { get; set; }

    public required long? UserId { get; set; }

    public required long? ShopId { get; set; }

    public required List<OrderLineResponse> OrderLines { get; set; }
}
