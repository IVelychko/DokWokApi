using Domain.DTOs.Requests.Products;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Products;

namespace Domain.Abstractions.Services;

public interface IProductService
{
    Task<IList<ProductResponse>> GetAllAsync();

    Task<ProductResponse> GetByIdAsync(long id);

    Task<ProductResponse> AddAsync(AddProductRequest request);

    Task<ProductResponse> UpdateAsync(UpdateProductRequest request);

    Task DeleteAsync(long id);
    
    Task<IList<ProductResponse>> GetAllByCategoryIdAsync(long categoryId);

    Task<IsTakenResponse> IsNameTakenAsync(string name);
}
