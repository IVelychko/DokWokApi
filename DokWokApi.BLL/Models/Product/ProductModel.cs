namespace DokWokApi.BLL.Models.Product;

public class ProductModel : BaseModel
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal Weight { get; set; }

    public string MeasurementUnit { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public long CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}
