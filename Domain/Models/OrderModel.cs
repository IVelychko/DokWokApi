namespace Domain.Models;

public class OrderModel : BaseModel
{
    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? DeliveryAddress { get; set; }

    public string PaymentType { get; set; } = string.Empty;

    public decimal TotalOrderPrice { get; set; }

    public DateTime CreationDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public long? ShopId { get; set; }

    public List<OrderLineModel> OrderLines { get; set; } = [];
}
