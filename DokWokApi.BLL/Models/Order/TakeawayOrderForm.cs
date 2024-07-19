using DokWokApi.BLL.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Order;

public class TakeawayOrderForm
{
    [Required]
    [RegularExpression(RegularExpressions.FirstName)]
    public string? CustomerName { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.PaymentType)]
    public string? PaymentType { get; set; }

    [GuidOrNull]
    public string? UserId { get; set; }

    public long? ShopId { get; set; }
}
