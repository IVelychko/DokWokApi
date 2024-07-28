using Domain.Models;
using Domain.ResultType;

namespace Domain.Abstractions.Services;

public interface IProductCategoryService : ICrud<ProductCategoryModel>
{
    Task<Result<bool>> IsNameTakenAsync(string name);
}
