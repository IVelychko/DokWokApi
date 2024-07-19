using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Validation;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.BLL.Validation;

public interface IUserServiceValidator : IValidator<UserModel>
{
    Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(UserPasswordChangeModel? model);

    Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel? model);

    Task<ValidationResult> ValidateCustomerLoginAsync(UserLoginModel? model);

    Task<ValidationResult> ValidateAdminLoginAsync(UserLoginModel? model);

    ValidationResult ValidateRefreshTokenModel(RefreshTokenModel? model);

    ValidationResult ValidateExpiredJwt(JwtSecurityToken? jwt, bool isAlgorithmValid);

    ValidationResult ValidateRefreshToken(RefreshToken? model, string jwtId);
}
