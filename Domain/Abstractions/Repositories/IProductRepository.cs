using Domain.Entities;
using Domain.Specifications.Products;

namespace Domain.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IList<Product>> GetAllBySpecificationAsync(ProductSpecification specification);
    
    Task<IList<Product>> GetAllByCategoryIdAsync(long categoryId);

    Task<Product?> GetBySpecificationAsync(ProductSpecification specification);

    Task<bool> IsNameUniqueAsync(string name);
    
    Task<bool> IsNameUniqueAsync(string name, long idToExclude);
    
    Task<bool> ProductExistsAsync(long id);
}
