using Domain.Helpers;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IProductService : ICrud<ProductModel>
{
    Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null);

    Task<Result<bool>> IsNameTakenAsync(string name);
}
