using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? DeliveryAddress { get; set; }

    public string PaymentType { get; set; } = string.Empty;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal TotalOrderPrice { get; set; }

    public DateTime CreationDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public long? UserId { get; set; }

    public long? ShopId { get; set; }

    public User? User { get; set; }

    public Shop? Shop { get; set; }

    public ICollection<OrderLine> OrderLines { get; set; } = [];
}
