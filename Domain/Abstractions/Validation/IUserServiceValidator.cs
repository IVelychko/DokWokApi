using FluentValidation.Results;
using Domain.DTOs.Requests.Users;

namespace Domain.Abstractions.Validation;

public interface IUserServiceValidator
{
    Task<ValidationResult> ValidateAddUserAsync(AddUserRequest request);
    
    Task<ValidationResult> ValidateDeleteUserAsync(DeleteUserRequest request);
    
    Task<ValidationResult> ValidateUpdatePasswordAsync(UpdatePasswordRequest request);
    
    Task<ValidationResult> ValidateUpdatePasswordAsAdminAsync(UpdatePasswordAsAdminRequest request);
    
    Task<ValidationResult> ValidateUpdateUserAsync(UpdateUserRequest request);
}
