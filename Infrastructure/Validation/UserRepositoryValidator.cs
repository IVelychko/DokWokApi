using Domain.Abstractions.Validation;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Mapping.Extensions;
using Infrastructure.Validation.Users.Add;
using Infrastructure.Validation.Users.Update;
using Infrastructure.Validation.Users.UpdatePassword;
using Infrastructure.Validation.Users.UpdatePasswordAsAdmin;

namespace Infrastructure.Validation;

public class UserRepositoryValidator : IUserRepositoryValidator
{
    private readonly IValidator<AddUserValidationModel> _addValidator;
    private readonly IValidator<UpdateUserValidationModel> _updateValidator;
    private readonly IValidator<UpdateUserPasswordValidationModel> _updatePasswordValidator;
    private readonly IValidator<UpdateUserPasswordAsAdminValidationModel> _updatePasswordAsAdminValidator;

    public UserRepositoryValidator(IValidator<AddUserValidationModel> addValidator,
        IValidator<UpdateUserValidationModel> updateValidator,
        IValidator<UpdateUserPasswordValidationModel> updatePasswordValidator,
        IValidator<UpdateUserPasswordAsAdminValidationModel> updatePasswordAsAdminValidator)
    {
        _addValidator = addValidator;
        _updateValidator = updateValidator;
        _updatePasswordValidator = updatePasswordValidator;
        _updatePasswordAsAdminValidator = updatePasswordAsAdminValidator;
    }

    public async Task<ValidationResult> ValidateAddAsync(ApplicationUser model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _addValidator.ValidateAsync(model.ToAddValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateAsync(ApplicationUser model)
    {
        if (model is null)
        {
            ValidationFailure[] failures = [new ValidationFailure(nameof(model), "The passed model is null")];
            return new(failures);
        }

        return await _updateValidator.ValidateAsync(model.ToUpdateValidationModel());
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(string userId, string newPassword)
    {
        return await _updatePasswordAsAdminValidator.ValidateAsync(new(userId, newPassword));
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        return await _updatePasswordValidator.ValidateAsync(new(userId, oldPassword, newPassword));
    }
}
