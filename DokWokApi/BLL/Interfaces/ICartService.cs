using DokWokApi.BLL.Models.ShoppingCart;

namespace DokWokApi.BLL.Interfaces;

public interface ICartService
{
    Task AddItem(long productId, int quantity);

    void ClearCart();

    Cart GetCart();

    Task RemoveItem(long productId, int quantity);

    Task RemoveLine(long productId);
}