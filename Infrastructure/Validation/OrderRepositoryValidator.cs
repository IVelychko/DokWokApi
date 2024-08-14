using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Mapping.Extensions;
using Infrastructure.Validation.Orders.Add;
using Infrastructure.Validation.Orders.Update;

namespace Infrastructure.Validation;

public class OrderRepositoryValidator : IOrderRepositoryValidator
{
    private readonly IValidator<AddOrderValidationModel> _addValidator;
    private readonly IValidator<UpdateOrderValidationModel> _updateValidator;

    public OrderRepositoryValidator(IValidator<AddOrderValidationModel> addValidator,
        IValidator<UpdateOrderValidationModel> updateValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(Order model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(model.ToAddValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateAsync(Order model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(model.ToUpdateValidationModel());
    }
}
