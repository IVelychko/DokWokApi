using DokWokApi.BLL.Models.ShoppingCart;
using LanguageExt.Common;

namespace DokWokApi.BLL.Interfaces;

public interface ICartService
{
    Task<Result<Cart?>> AddItem(long productId, int quantity);

    Task<bool> ClearCart();

    Task<Cart?> GetCart();

    Task<Result<Cart?>> RemoveItem(long productId, int quantity);

    Task<Result<Cart?>> RemoveLine(long productId);
}