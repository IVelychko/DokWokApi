using DokWokApi.DAL.Entities;

namespace DokWokApi.DAL.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    IQueryable<Product> GetAllWithDetails();

    Task<Product?> GetByIdWithDetailsAsync(long id);
}
