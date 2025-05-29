using Domain.DTOs.Requests.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;

namespace Domain.Abstractions.Services;

public interface IProductCategoryService
{
    Task<IList<ProductCategoryResponse>> GetAllAsync();

    Task<ProductCategoryResponse> GetByIdAsync(long id);

    Task<ProductCategoryResponse> AddAsync(AddProductCategoryRequest request);

    Task<ProductCategoryResponse> UpdateAsync(UpdateProductCategoryRequest request);

    Task DeleteAsync(long id);
    
    Task<bool> IsNameUniqueAsync(string name);
}
