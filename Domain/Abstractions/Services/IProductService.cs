using Domain.DTOs.Commands.Products;
using Domain.DTOs.Responses.Products;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync(PageInfo? pageInfo = null);

    Task<ProductResponse?> GetByIdAsync(long id);

    Task<ProductResponse> AddAsync(AddProductCommand command);

    Task<ProductResponse> UpdateAsync(UpdateProductCommand command);

    Task DeleteAsync(long id);
    
    Task<IEnumerable<ProductResponse>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null);

    Task<bool> IsNameUniqueAsync(string name);
}
