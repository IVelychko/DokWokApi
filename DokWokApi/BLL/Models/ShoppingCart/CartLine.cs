namespace DokWokApi.BLL.Models.ShoppingCart;

public class CartLine
{
    public ProductModel Product { get; set; } = new();

    public int Quantity { get; set; }

    public decimal TotalLinePrice { get; set; }

    public void CalculateTotalLinePrice() => TotalLinePrice = Product.Price * Quantity;
}
