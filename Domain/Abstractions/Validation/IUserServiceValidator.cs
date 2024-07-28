using Domain.Entities;
using Domain.Models.User;
using Domain.Validation;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Abstractions.Validation;

public interface IUserServiceValidator
{
    Task<ValidationResult> ValidateCustomerLoginAsync(string? userName, string? password);

    Task<ValidationResult> ValidateAdminLoginAsync(string? userName, string? password);

    ValidationResult ValidateRefreshTokenModel(string? securityToken, string? refreshToken);

    ValidationResult ValidateExpiredJwt(JwtSecurityToken? jwt, bool isAlgorithmValid);

    ValidationResult ValidateRefreshToken(RefreshToken? model, string? jwtId);
}
