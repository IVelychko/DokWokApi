namespace DokWokApi.BLL.Models;

public class OrderLineModel : BaseModel
{
    public int Quantity { get; set; }

    public decimal TotalLinePrice { get; set; }

    public ProductModel? Product { get; set; }
}
