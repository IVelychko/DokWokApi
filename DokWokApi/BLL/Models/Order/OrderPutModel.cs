using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Order;

public class OrderPutModel
{
    [Required]
    public long? Id { get; set; }

    [Required]
    public string? CustomerName { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Email { get; set; }

    [Required]
    public string? DeliveryAddress { get; set; }

    [Required]
    public string? PaymentType { get; set; }

    [Required]
    public decimal? TotalOrderPrice { get; set; }

    [Required]
    public DateTime? CreationDate { get; set; }

    [Required]
    public string? Status { get; set; }

    public string? UserId { get; set; }
}
