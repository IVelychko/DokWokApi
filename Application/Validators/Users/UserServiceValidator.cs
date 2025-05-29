using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Users;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.Users;

public class UserServiceValidator : IUserServiceValidator
{
    private readonly IValidator<AddUserRequest> _addUserValidator;
    private readonly IValidator<DeleteUserRequest> _deleteUserValidator;
    private readonly IValidator<UpdatePasswordRequest> _updatePasswordValidator;
    private readonly IValidator<UpdatePasswordAsAdminRequest> _updatePasswordAsAdminValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserValidator;


    public UserServiceValidator(
        IValidator<AddUserRequest> addUserValidator,
        IValidator<DeleteUserRequest> deleteUserValidator,
        IValidator<UpdatePasswordRequest> updatePasswordValidator,
        IValidator<UpdatePasswordAsAdminRequest> updatePasswordAsAdminValidator,
        IValidator<UpdateUserRequest> updateUserValidator)
    {
        _addUserValidator = addUserValidator;
        _deleteUserValidator = deleteUserValidator;
        _updatePasswordValidator = updatePasswordValidator;
        _updatePasswordAsAdminValidator = updatePasswordAsAdminValidator;
        _updateUserValidator = updateUserValidator;
    }

    public async Task<ValidationResult> ValidateAddUserAsync(AddUserRequest request)
    {
        return await _addUserValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateDeleteUserAsync(DeleteUserRequest request)
    {
        return await _deleteUserValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateUpdatePasswordAsync(UpdatePasswordRequest request)
    {
        return await _updatePasswordValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateUpdatePasswordAsAdminAsync(UpdatePasswordAsAdminRequest request)
    {
        return await _updatePasswordAsAdminValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateUpdateUserAsync(UpdateUserRequest request)
    {
        return await _updateUserValidator.ValidateAsync(request);
    }
}
