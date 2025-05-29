using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Products;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.Products;

public class ProductServiceValidator : IProductServiceValidator
{
    private readonly IValidator<UpdateProductRequest> _updateProductValidator;
    private readonly IValidator<AddProductRequest> _addProductValidator;
    private readonly IValidator<DeleteProductRequest> _deleteProductValidator;

    public ProductServiceValidator(
        IValidator<UpdateProductRequest> updateProductValidator,
        IValidator<AddProductRequest> addProductValidator,
        IValidator<DeleteProductRequest> deleteProductValidator)
    {
        _updateProductValidator = updateProductValidator;
        _addProductValidator = addProductValidator;
        _deleteProductValidator = deleteProductValidator;
    }
    
    public async Task<ValidationResult> ValidateUpdateProductAsync(UpdateProductRequest request)
    {
        return await _updateProductValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateAddProductAsync(AddProductRequest request)
    {
        return await _addProductValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateDeleteProductAsync(DeleteProductRequest request)
    {
        return await _deleteProductValidator.ValidateAsync(request);
    }
}