using Domain.Entities;
using Domain.Helpers;

namespace Domain.Abstractions.Repositories;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<Result<bool>> IsNameTakenAsync(string name);
}
