using DokWokApi.BLL.Models.ShoppingCart;

namespace DokWokApi.BLL.Interfaces;

public interface ICartService
{
    Task<Cart> AddItem(long productId, int quantity);

    Task ClearCart();

    Task<Cart> GetCart();

    Task<Cart> RemoveItem(long productId, int quantity);

    Task<Cart> RemoveLine(long productId);
}