using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.DAL.ResultType;

namespace DokWokApi.BLL.Interfaces;

public interface IProductCategoryService : ICrud<ProductCategoryModel>
{
    Task<Result<bool>> IsNameTaken(string name);
}
