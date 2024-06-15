using DokWokApi.BLL.Models.Shop;

namespace DokWokApi.BLL.Interfaces;

public interface IShopService : ICrud<ShopModel>
{
    Task<ShopModel?> GetByAddressAsync(string street, string building);

    Task<bool> IsAddressTaken(string street, string building);
}
