using DokWokApi.BLL.Models.Shop;
using LanguageExt.Common;

namespace DokWokApi.BLL.Interfaces;

public interface IShopService : ICrud<ShopModel>
{
    Task<ShopModel?> GetByAddressAsync(string street, string building);

    Task<Result<bool>> IsAddressTaken(string street, string building);
}
