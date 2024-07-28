using Domain.Models;
using Domain.ResultType;

namespace Domain.Abstractions.Services;

public interface IProductService : ICrud<ProductModel>
{
    Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId);

    Task<Result<bool>> IsNameTakenAsync(string name);
}
