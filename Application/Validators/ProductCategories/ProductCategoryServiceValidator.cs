using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.ProductCategories;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.ProductCategories;

public class ProductCategoryServiceValidator : IProductCategoryServiceValidator
{
    private readonly IValidator<UpdateProductCategoryRequest> _updateCategoryValidator;
    private readonly IValidator<AddProductCategoryRequest> _addCategoryValidator;
    private readonly IValidator<DeleteProductCategoryRequest> _deleteCategoryValidator;

    public ProductCategoryServiceValidator(
        IValidator<UpdateProductCategoryRequest> updateCategoryValidator,
        IValidator<AddProductCategoryRequest> addCategoryValidator,
        IValidator<DeleteProductCategoryRequest> deleteCategoryValidator)
    {
        _updateCategoryValidator = updateCategoryValidator;
        _addCategoryValidator = addCategoryValidator;
        _deleteCategoryValidator = deleteCategoryValidator;
    }
    
    public async Task<ValidationResult> ValidateUpdateCategoryAsync(UpdateProductCategoryRequest request)
    {
        return await _updateCategoryValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateAddCategoryAsync(AddProductCategoryRequest request)
    {
        return await _addCategoryValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateDeleteCategoryAsync(DeleteProductCategoryRequest request)
    {
        return await _deleteCategoryValidator.ValidateAsync(request);
    }
}