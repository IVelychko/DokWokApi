using DokWokApi.DAL.Entities;

namespace DokWokApi.DAL.Interfaces;

public interface IShopRepository : IRepository<Shop>
{
    Task<Shop?> GetByAddressAsync(string street, string building);
}
