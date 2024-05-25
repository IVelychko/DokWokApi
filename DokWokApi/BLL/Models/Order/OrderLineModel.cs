using DokWokApi.BLL.Models.Product;

namespace DokWokApi.BLL.Models.Order;

public class OrderLineModel : BaseModel
{
    public int Quantity { get; set; }

    public decimal TotalLinePrice { get; set; }

    public ProductModel? Product { get; set; }
}
