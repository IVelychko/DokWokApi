using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace Domain.Validation;

public class UserServiceValidator : IUserServiceValidator
{
    private readonly IUserRepository _userRepository;

    public UserServiceValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ValidationResult> ValidateAdminLoginAsync(string? userName, string? password)
    {
        ValidationResult result = new(true);
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Errors.Add("The passed data is null.");
            return result;
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            result.IsValid = false;
            result.Errors.Add("The credentials are wrong.");
            return result;
        }

        var isAdminResult = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdminResult.IsFaulted)
        {
            result.IsValid = false;
            result.Errors.AddRange(isAdminResult.Error.Errors);
            return result;
        }

        if (!isAdminResult.Value)
        {
            result.IsValid = false;
            result.Errors.Add("The authorization is denied");
            return result;
        }

        var isValidPassword = await _userRepository.CheckUserPasswordAsync(user, password);
        if (!isValidPassword)
        {
            result.IsValid = false;
            result.Errors.Add("The credentials are wrong");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateCustomerLoginAsync(string? userName, string? password)
    {
        ValidationResult result = new(true);
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Errors.Add("The passed data is null");
            return result;
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            result.IsValid = false;
            result.Errors.Add("The credentials are wrong");
            return result;
        }

        var isCustomerResult = await _userRepository.IsInRoleAsync(user, UserRoles.Customer);
        if (isCustomerResult.IsFaulted)
        {
            result.IsValid = false;
            result.Errors.AddRange(isCustomerResult.Error.Errors);
            return result;
        }

        var isAdminResult = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdminResult.IsFaulted)
        {
            result.IsValid = false;
            result.Errors.AddRange(isAdminResult.Error.Errors);
            return result;
        }

        if (!(isCustomerResult.Value || isAdminResult.Value))
        {
            result.IsValid = false;
            result.Errors.Add("The authorization is denied");
            return result;
        }

        var isValidPassword = await _userRepository.CheckUserPasswordAsync(user, password);
        if (!isValidPassword)
        {
            result.IsValid = false;
            result.Errors.Add("The credentials are wrong");
        }

        return result;
    }

    public ValidationResult ValidateRefreshTokenModel(string? securityToken, string? refreshToken)
    {
        ValidationResult result = new(true);
        if (string.IsNullOrEmpty(securityToken) || string.IsNullOrEmpty(refreshToken))
        {
            result.IsValid = false;
            result.Errors.Add("The passed data is null");
        }

        return result;
    }

    public ValidationResult ValidateExpiredJwt(JwtSecurityToken? jwt, bool isAlgorithmValid)
    {
        ValidationResult result = new(true);
        if (jwt is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed JWT is null");
            return result;
        }

        if (!isAlgorithmValid)
        {
            result.IsValid = false;
            result.Errors.Add("The token is not valid");
        }
        if (jwt.ValidTo > DateTime.UtcNow)
        {
            result.IsValid = false;
            result.Errors.Add("The token has not expired yet");
        }

        return result;
    }

    public ValidationResult ValidateRefreshToken(RefreshToken? model, string? jwtId)
    {
        ValidationResult result = new(true);
        if (model is null || string.IsNullOrEmpty(jwtId))
        {
            result.IsValid = false;
            if (model is null)
            {
                result.Errors.Add("The passed refresh token is null");
            }

            if (string.IsNullOrEmpty(jwtId))
            {
                result.Errors.Add("The passed Jwt Id is null");
            }

            return result;
        }

        if (DateTime.UtcNow > model.ExpiryDate)
        {
            result.IsValid = false;
            result.Errors.Add("The refresh token has expired");
        }

        if (model.Invalidated)
        {
            result.IsValid = false;
            result.Errors.Add("The refresh token has been invalidated");
        }

        if (model.Used)
        {
            result.IsValid = false;
            result.Errors.Add("The refresh token has been used");
        }

        if (model.JwtId != jwtId)
        {
            result.IsValid = false;
            result.Errors.Add("The refresh token does not match the JWT");
        }

        return result;
    }
}
