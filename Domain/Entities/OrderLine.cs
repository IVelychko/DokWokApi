namespace Domain.Entities;

public class OrderLine : BaseEntity
{
    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }
    
    public decimal TotalLinePrice { get; set; }

    public Order? Order { get; set; }

    public Product? Product { get; set; }
}
