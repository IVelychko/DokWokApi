using Domain.Models;

namespace Application.Operations.Order;

public class OrderResponse
{
    public required long Id { get; set; }

    public required string CustomerName { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Email { get; set; }

    public string? DeliveryAddress { get; set; }

    public required string PaymentType { get; set; }

    public required decimal TotalOrderPrice { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string Status { get; set; }

    public string? UserId { get; set; }

    public long? ShopId { get; set; }

    public List<OrderLineModel> OrderLines { get; set; } = [];
}
