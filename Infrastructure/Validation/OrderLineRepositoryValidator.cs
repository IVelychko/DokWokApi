using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Mapping.Extensions;
using Infrastructure.Validation.OrderLines.Add;
using Infrastructure.Validation.OrderLines.Update;

namespace Infrastructure.Validation;

public class OrderLineRepositoryValidator : IOrderLineRepositoryValidator
{
    private readonly IValidator<AddOrderLineValidationModel> _addValidator;
    private readonly IValidator<UpdateOrderLineValidationModel> _updateValidator;

    public OrderLineRepositoryValidator(IValidator<AddOrderLineValidationModel> addValidator,
        IValidator<UpdateOrderLineValidationModel> updateValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(OrderLine model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(model.ToAddValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateAsync(OrderLine model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(model.ToUpdateValidationModel());
    }
}
