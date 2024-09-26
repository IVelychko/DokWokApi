using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Validation.Users.ExpiredJwt;
using Domain.Validation.Users.RefreshToken;
using FluentValidation;
using FluentValidation.Results;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Validation;

public class UserServiceValidator : IUserServiceValidator
{
    private readonly IValidator<ExpiredJwtValidationModel> _expiredJwtValidator;
    private readonly IValidator<RefreshTokenValidationModel> _refreshTokenValidator;


    public UserServiceValidator(IValidator<ExpiredJwtValidationModel> expiredJwtValidator,
        IValidator<RefreshTokenValidationModel?> refreshTokenValidator)
    {
        _expiredJwtValidator = expiredJwtValidator;
        _refreshTokenValidator = refreshTokenValidator;
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
