using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<bool> IsNameUniqueAsync(string name);
    
    Task<bool> IsNameUniqueAsync(string name, long idToExclude);
    
    Task<bool> CategoryExistsAsync(long id);
}
