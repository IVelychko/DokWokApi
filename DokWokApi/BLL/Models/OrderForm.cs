namespace DokWokApi.BLL.Models;

public class OrderForm
{
    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;

    public string PaymentType { get; set; } = string.Empty;

    public string? UserId { get; set; }
}
