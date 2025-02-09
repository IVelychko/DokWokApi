using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<bool> IsNameUniqueAsync(string name);
}
