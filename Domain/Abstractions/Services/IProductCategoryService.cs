using Domain.DTOs.Commands.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IProductCategoryService
{
    Task<IEnumerable<ProductCategoryResponse>> GetAllAsync(PageInfo? pageInfo = null);

    Task<ProductCategoryResponse?> GetByIdAsync(long id);

    Task<ProductCategoryResponse> AddAsync(AddProductCategoryCommand command);

    Task<ProductCategoryResponse> UpdateAsync(UpdateProductCategoryCommand command);

    Task DeleteAsync(long id);
    
    Task<bool> IsNameUniqueAsync(string name);
}
