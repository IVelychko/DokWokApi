using System.ComponentModel.DataAnnotations.Schema;

namespace DokWokApi.DAL.Entities;

public class OrderLine : BaseEntity
{
    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(8, 2)")]
    public decimal TotalLinePrice { get; set; }

    public Order? Order { get; set; }

    public Product? Product { get; set; }
}
