using Domain.Entities;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<Result<bool>> IsNameTakenAsync(string name);
}
