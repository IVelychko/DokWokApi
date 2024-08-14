using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Mapping.Extensions;
using Infrastructure.Validation.ProductCategories.Add;
using Infrastructure.Validation.ProductCategories.Update;

namespace Infrastructure.Validation;

public class ProductCategoryRepositoryValidator : IProductCategoryRepositoryValidator
{
    private readonly IValidator<AddProductCategoryValidationModel> _addValidator;
    private readonly IValidator<UpdateProductCategoryValidationModel> _updateValidator;

    public ProductCategoryRepositoryValidator(IValidator<AddProductCategoryValidationModel> addValidator,
        IValidator<UpdateProductCategoryValidationModel> updateValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(ProductCategory model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(model.ToAddValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateAsync(ProductCategory model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(model.ToUpdateValidationModel());
    }
}
