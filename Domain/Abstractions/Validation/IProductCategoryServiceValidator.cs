using Domain.DTOs.Requests.ProductCategories;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IProductCategoryServiceValidator
{
    Task<ValidationResult> ValidateUpdateCategoryAsync(UpdateProductCategoryRequest request);
    
    Task<ValidationResult> ValidateAddCategoryAsync(AddProductCategoryRequest request);
    
    Task<ValidationResult> ValidateDeleteCategoryAsync(DeleteProductCategoryRequest request);
}