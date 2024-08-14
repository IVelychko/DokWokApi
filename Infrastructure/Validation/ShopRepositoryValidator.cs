using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Mapping.Extensions;
using Infrastructure.Validation.Shops.Add;
using Infrastructure.Validation.Shops.Update;

namespace Infrastructure.Validation;

public class ShopRepositoryValidator : IShopRepositoryValidator
{
    private readonly IValidator<AddShopValidationModel> _addValidator;
    private readonly IValidator<UpdateShopValidationModel> _updateValidator;

    public ShopRepositoryValidator(IValidator<AddShopValidationModel> addValidator, IValidator<UpdateShopValidationModel> updateValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(Shop model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(model.ToAddValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Shop model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(model.ToUpdateValidationModel());
    }
}
