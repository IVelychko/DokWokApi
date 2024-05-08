namespace DokWokApi.BLL.Models.ShoppingCart;

public class Cart
{
    public List<CartLine> Lines { get; set; } = [];

    public decimal TotalCartPrice { get; set; }

    public void CalculateTotalCartPrice() => TotalCartPrice = Lines.Sum(cl => cl.TotalLinePrice);
}
