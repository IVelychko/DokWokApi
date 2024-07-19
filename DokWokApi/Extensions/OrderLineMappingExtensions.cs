using DokWokApi.BLL.Models.Order;
using DokWokApi.Models.ShoppingCart;

namespace DokWokApi.Extensions;

public static class OrderLineMappingExtensions
{
    public static OrderLineModel ToOrderLineModel(this CartLine model)
    {
        return new()
        {
            Quantity = model.Quantity,
            TotalLinePrice = model.TotalLinePrice,
            ProductId = model.Product is not null ? model.Product.Id : 0,
            Product = model.Product
        };
    }
}
