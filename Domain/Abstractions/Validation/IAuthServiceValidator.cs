using Domain.DTOs.Requests.Users;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IAuthServiceValidator
{
    Task<ValidationResult> ValidateLoginAdminUserAsync(LoginAdminRequest request);
    
    Task<ValidationResult> ValidateLoginCustomerUserAsync(LoginCustomerRequest request);
    
    Task<ValidationResult> ValidateLogOutUserAsync(LogOutUserRequest request);
    
    Task<ValidationResult> ValidateRefreshUserTokenAsync(RefreshUserTokenRequest request);
    
    Task<ValidationResult> ValidateRegisterUserAsync(RegisterUserRequest request);
}