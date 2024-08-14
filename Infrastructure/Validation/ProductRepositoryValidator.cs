using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Mapping.Extensions;
using Infrastructure.Validation.Products.Add;
using Infrastructure.Validation.Products.Update;

namespace Infrastructure.Validation;

public class ProductRepositoryValidator : IProductRepositoryValidator
{
    private readonly IValidator<AddProductValidationModel> _addValidator;
    private readonly IValidator<UpdateProductValidationModel> _updateValidator;

    public ProductRepositoryValidator(IValidator<AddProductValidationModel> addValidator,
        IValidator<UpdateProductValidationModel> updateValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(Product model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(model.ToAddValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Product model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(model.ToUpdateValidationModel());
    }
}
