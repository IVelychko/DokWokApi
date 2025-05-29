using Domain.DTOs.Requests.Products;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IProductServiceValidator
{
    Task<ValidationResult> ValidateUpdateProductAsync(UpdateProductRequest request);
    
    Task<ValidationResult> ValidateAddProductAsync(AddProductRequest request);
    
    Task<ValidationResult> ValidateDeleteProductAsync(DeleteProductRequest request);
}