using System.ComponentModel.DataAnnotations.Schema;

namespace DokWokApi.DAL.Entities;

public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DeliveryAddress { get; set; } = string.Empty;

    public string PaymentType { get; set; } = string.Empty;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal TotalOrderPrice { get; set; }

    public DateTime CreationDate { get; set; }

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<OrderLine> OrderLines { get; set; } = [];
}
