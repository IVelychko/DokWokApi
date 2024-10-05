using Domain.Helpers;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IProductCategoryService : ICrud<ProductCategoryModel>
{
    Task<Result<bool>> IsNameTakenAsync(string name);
}
