using DokWokApi.BLL.Attributes;
using DokWokApi.BLL.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DokWokApi.BLL.Models.Order;

public class DeliveryOrderModel
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
    [RegularExpression(RegularExpressions.Address)]
    public string? DeliveryAddress { get; set; }

    [Required]
    [RegularExpression(RegularExpressions.PaymentType)]
    public string? PaymentType { get; set; }

    [GuidOrNull]
    public string? UserId { get; set; }
}
