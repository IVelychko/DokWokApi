using Domain.DTOs.Requests.Shops;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Shops;

namespace Domain.Abstractions.Services;

public interface IShopService
{
    Task<IList<ShopResponse>> GetAllAsync();

    Task<ShopResponse> GetByIdAsync(long id);

    Task<ShopResponse> AddAsync(AddShopRequest request);

    Task<ShopResponse> UpdateAsync(UpdateShopRequest request);

    Task DeleteAsync(long id);
    
    Task<ShopResponse> GetByAddressAsync(string street, string building);

    Task<IsTakenResponse> IsAddressTakenAsync(string street, string building);
}
