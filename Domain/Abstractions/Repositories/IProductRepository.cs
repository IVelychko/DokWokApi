using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IList<Product>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IList<Product>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null);

    Task<IList<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null);

    Task<Product?> GetByIdWithDetailsAsync(long id);

    Task<bool> IsNameUniqueAsync(string name);
    
    Task<bool> IsNameUniqueAsync(string name, long idToExclude);
    
    Task<bool> ProductExistsAsync(long id);
}
