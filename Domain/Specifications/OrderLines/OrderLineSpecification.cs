namespace Domain.Specifications.OrderLines;

public class OrderLineSpecification
{
    public bool IncludeProduct { get; set; }

    public bool IncludeCategory { get; set; }
    
    public long? OrderId { get; set; }
    
    public static OrderLineSpecification IncludeAll => new()
    {
        IncludeProduct = true,
        IncludeCategory = true
    };
}