using DokWokApi.BLL.Models.Product;
using LanguageExt.Common;

namespace DokWokApi.BLL.Interfaces;

public interface IProductService : ICrud<ProductModel>
{
    Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId);

    Task<Result<bool>> IsNameTaken(string name);
}
