using Domain.Shared;

namespace Domain.Specifications.Products;

public class ProductSpecification
{
    public bool IncludeCategory { get; set; }
    
    public long? Id { get; set; }
    
    public long? CategoryId { get; set; }
    
    public decimal? MinPrice { get; set; }
    
    public decimal? MaxPrice { get; set; }
    
    public PageInfo? PageInfo { get; set; }
    
    public static ProductSpecification IncludeAll => new()
    {
        IncludeCategory = true
    };
}