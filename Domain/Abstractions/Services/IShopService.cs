using Domain.Helpers;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IShopService : ICrud<ShopModel>
{
    Task<ShopModel?> GetByAddressAsync(string street, string building);

    Task<Result<bool>> IsAddressTakenAsync(string street, string building);
}
