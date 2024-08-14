using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Validation.RefreshTokens.Add;
using Infrastructure.Validation.RefreshTokens.Update;

namespace Infrastructure.Validation;

public class RefreshTokenRepositoryValidator : IRefreshTokenRepositoryValidator
{
    private readonly IValidator<AddRefreshTokenValidationModel> _addValidator;
    private readonly IValidator<UpdateRefreshTokenValidationModel> _updateValidator;

    public RefreshTokenRepositoryValidator(IValidator<AddRefreshTokenValidationModel> addValidator,
        IValidator<UpdateRefreshTokenValidationModel> updateValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(RefreshToken model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(new(model.UserId));
    }

    public async Task<ValidationResult> ValidateUpdateAsync(RefreshToken model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(new(model.Id, model.UserId));
    }
}
