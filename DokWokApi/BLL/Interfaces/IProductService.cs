using DokWokApi.BLL.Models.Product;

namespace DokWokApi.BLL.Interfaces;

public interface IProductService : ICrud<ProductModel>
{
    Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId);
}
