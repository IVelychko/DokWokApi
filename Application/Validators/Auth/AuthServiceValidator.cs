using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Users;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.Auth;

public class AuthServiceValidator : IAuthServiceValidator
{
    private readonly IValidator<LoginAdminRequest> _loginAdminValidator;
    private readonly IValidator<LoginCustomerRequest> _loginCustomerValidator;
    private readonly IValidator<LogOutUserRequest> _logOutUserValidator;
    private readonly IValidator<RefreshUserTokenRequest> _refreshUserTokenValidator;
    private readonly IValidator<RegisterUserRequest> _registerUserValidator;

    public AuthServiceValidator(
        IValidator<LoginAdminRequest> loginAdminValidator,
        IValidator<LoginCustomerRequest> loginCustomerValidator,
        IValidator<LogOutUserRequest> logOutUserValidator,
        IValidator<RefreshUserTokenRequest> refreshUserTokenValidator,
        IValidator<RegisterUserRequest> registerUserValidator)
    {
        _loginAdminValidator = loginAdminValidator;
        _loginCustomerValidator = loginCustomerValidator;
        _logOutUserValidator = logOutUserValidator;
        _refreshUserTokenValidator = refreshUserTokenValidator;
        _registerUserValidator = registerUserValidator;
    }
    
    public async Task<ValidationResult> ValidateLoginAdminUserAsync(LoginAdminRequest request)
    {
        return await _loginAdminValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateLoginCustomerUserAsync(LoginCustomerRequest request)
    {
        return await _loginCustomerValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateLogOutUserAsync(LogOutUserRequest request)
    {
        return await _logOutUserValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateRefreshUserTokenAsync(RefreshUserTokenRequest request)
    {
        return await _refreshUserTokenValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateRegisterUserAsync(RegisterUserRequest request)
    {
        return await _registerUserValidator.ValidateAsync(request);
    }
}