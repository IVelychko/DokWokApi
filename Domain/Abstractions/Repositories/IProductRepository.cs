using Domain.Entities;
using Domain.Helpers;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<Product>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null);

    Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null);

    Task<Product?> GetByIdWithDetailsAsync(long id);

    Task<Result<bool>> IsNameTakenAsync(string name);
}
