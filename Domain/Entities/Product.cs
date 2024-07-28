using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(8, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(8, 2)")]
    public decimal Weight { get; set; }

    public string MeasurementUnit { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public long CategoryId { get; set; }

    public ProductCategory? Category { get; set; }

    public ICollection<OrderLine> OrderLines { get; set; } = [];
}
