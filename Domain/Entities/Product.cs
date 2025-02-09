namespace Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public decimal Weight { get; set; }

    public string MeasurementUnit { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public long CategoryId { get; set; }

    public ProductCategory? Category { get; set; }

    public ICollection<OrderLine> OrderLines { get; set; } = [];
}
