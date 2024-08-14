using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Validation.Users.AdminLogin;
using Domain.Validation.Users.CustomerLogin;
using Domain.Validation.Users.ExpiredJwt;
using Domain.Validation.Users.RefreshToken;
using FluentValidation;
using FluentValidation.Results;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Validation;

public class UserServiceValidator : IUserServiceValidator
{
    private readonly IValidator<AdminLoginValidationModel> _adminLoginValidator;
    private readonly IValidator<CustomerLoginValidationModel> _customerValidator;
    private readonly IValidator<ExpiredJwtValidationModel> _expiredJwtValidator;
    private readonly IValidator<RefreshTokenValidationModel> _refreshTokenValidator;


    public UserServiceValidator(IValidator<AdminLoginValidationModel> adminLoginValidator,
        IValidator<CustomerLoginValidationModel> customerValidator,
        IValidator<ExpiredJwtValidationModel> expiredJwtValidator,
        IValidator<RefreshTokenValidationModel?> refreshTokenValidator)
    {
        _adminLoginValidator = adminLoginValidator;
        _customerValidator = customerValidator;
        _expiredJwtValidator = expiredJwtValidator;
        _refreshTokenValidator = refreshTokenValidator;
    }

    public async Task<ValidationResult> ValidateAdminLoginAsync(string userName, string password)
    {
        return await _adminLoginValidator.ValidateAsync(new(userName, password));
    }

    public async Task<ValidationResult> ValidateCustomerLoginAsync(string userName, string password)
    {
        return await _customerValidator.ValidateAsync(new(userName, password));
    }

    public ValidationResult ValidateExpiredJwt(JwtSecurityToken jwt, bool isAlgorithmValid)
    {
        //return _expiredJwtValidator.Validate(new(null!, isAlgorithmValid));
        return _expiredJwtValidator.Validate(new(jwt, isAlgorithmValid));
    }

    public ValidationResult ValidateRefreshToken(RefreshToken model, string jwtId)
    {
        return _refreshTokenValidator.Validate(new(model, jwtId));
    }
}
