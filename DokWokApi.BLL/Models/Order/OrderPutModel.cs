using DokWokApi.BLL.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Order;

public class OrderPutModel
{
    [Required]
    [Range(0, long.MaxValue)]
    public long? Id { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.FirstName)]
    public string? CustomerName { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [AddressOrNull]
    public string? DeliveryAddress { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.PaymentType)]
    public string? PaymentType { get; set; }

    [Required]
    [Range(0, 1_000_000)]
    public decimal? TotalOrderPrice { get; set; }

    [Required]
    public DateTime? CreationDate { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.OrderStatus)]
    public string? Status { get; set; }

    [GuidOrNull]
    public string? UserId { get; set; }

    public long? ShopId { get; set; }
}
