using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IShopRepository : IRepository<Shop>
{
    Task<Shop?> GetByAddressAsync(string street, string building);

    Task<bool> IsAddressUniqueAsync(string street, string building);
    
    Task<bool> IsAddressUniqueAsync(string street, string building, long idToExclude);
    
    Task<bool> ShopExistsAsync(long id);
}
