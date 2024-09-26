using Domain.Entities;
using FluentValidation.Results;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Abstractions.Validation;

public interface IUserServiceValidator
{
    ValidationResult ValidateExpiredJwt(JwtSecurityToken jwt, bool isAlgorithmValid);

    ValidationResult ValidateRefreshToken(RefreshToken model, string jwtId);
}
