using Domain.Entities;
using Domain.Helpers;

namespace Domain.Abstractions.Repositories;

public interface IShopRepository : IRepository<Shop>
{
    Task<Shop?> GetByAddressAsync(string street, string building);

    Task<Result<bool>> IsAddressTakenAsync(string street, string building);
}
