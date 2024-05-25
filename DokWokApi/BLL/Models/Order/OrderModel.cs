namespace DokWokApi.BLL.Models.Order;

public class OrderModel : BaseModel
{
    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;

    public string PaymentType { get; set; } = string.Empty;

    public decimal TotalOrderPrice { get; set; }

    public DateTime CreationDate { get; set; }

    public bool IsCheckedOut { get; set; }

    public string? UserId { get; set; }

    public List<OrderLineModel> OrderLines { get; set; } = [];
}
