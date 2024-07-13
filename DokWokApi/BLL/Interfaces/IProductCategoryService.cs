using DokWokApi.BLL.Models.ProductCategory;
using LanguageExt.Common;

namespace DokWokApi.BLL.Interfaces;

public interface IProductCategoryService : ICrud<ProductCategoryModel>
{
    Task<Result<bool>> IsNameTaken(string name);
}
