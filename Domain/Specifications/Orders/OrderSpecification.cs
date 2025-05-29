namespace Domain.Specifications.Orders;

public class OrderSpecification
{
    public long? Id { get; set; }
    
    public long? UserId { get; set; }
    
    public bool IncludeOrderLines { get; set; }
    
    public bool IncludeProduct { get; set; }

    public bool IncludeCategory { get; set; }
    
    public static OrderSpecification IncludeAll => new()
    {
        IncludeOrderLines = true,
        IncludeProduct = true,
        IncludeCategory = true
    };
}