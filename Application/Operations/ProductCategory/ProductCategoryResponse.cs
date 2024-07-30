namespace Application.Operations.ProductCategory;

public class ProductCategoryResponse : BaseResponse<long>
{
    //public required long Id { get; set; }

    public required string Name { get; set; }
}
