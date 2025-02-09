using Domain.Entities;
using Domain.Shared;

namespace Domain.Abstractions.Repositories;

public interface IShopRepository : IRepository<Shop>
{
    Task<Shop?> GetByAddressAsync(string street, string building);

    Task<bool> IsAddressUniqueAsync(string street, string building);
}
