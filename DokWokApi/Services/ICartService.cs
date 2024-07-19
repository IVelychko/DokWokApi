using DokWokApi.Models.ShoppingCart;
using DokWokApi.DAL.ResultType;

namespace DokWokApi.Services;

public interface ICartService
{
    Task<Result<Cart?>> AddItem(long productId, int quantity);

    Task<bool> ClearCart();

    Task<Cart?> GetCart();

    Task<Result<Cart?>> RemoveItem(long productId, int quantity);

    Task<Result<Cart?>> RemoveLine(long productId);
}