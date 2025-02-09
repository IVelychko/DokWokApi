using Domain.DTOs.Commands.Shops;
using Domain.DTOs.Responses.Shops;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IShopService
{
    Task<IEnumerable<ShopResponse>> GetAllAsync(PageInfo? pageInfo = null);

    Task<ShopResponse?> GetByIdAsync(long id);

    Task<ShopResponse> AddAsync(AddShopCommand command);

    Task<ShopResponse> UpdateAsync(UpdateShopCommand command);

    Task DeleteAsync(long id);
    
    Task<ShopResponse?> GetByAddressAsync(string street, string building);

    Task<bool> IsAddressUniqueAsync(string street, string building);
}
