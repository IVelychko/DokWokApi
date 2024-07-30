using Domain.Models;

namespace Application.Operations.Order;

public class OrderResponse : BaseResponse<long>
{
    //public required long Id { get; set; }

    public required string CustomerName { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Email { get; set; }

    public required string? DeliveryAddress { get; set; }

    public required string PaymentType { get; set; }

    public required decimal TotalOrderPrice { get; set; }

    public required DateTime CreationDate { get; set; }

    public required string Status { get; set; }

    public required string? UserId { get; set; }

    public required long? ShopId { get; set; }

    public required List<OrderLineModel> OrderLines { get; set; }
}
