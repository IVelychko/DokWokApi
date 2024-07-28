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
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Error = "The passed data is null.";
            return result;
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        bool isAdmin = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        if (!isAdmin)
        {
            result.IsValid = false;
            result.Error = "The authorization is denied.";
            return result;
        }

        var isValidPassword = await _userRepository.CheckUserPasswordAsync(user, password);
        if (!isValidPassword)
        {
            result.IsValid = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateCustomerLoginAsync(string? userName, string? password)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Error = "The passed data is null.";
            return result;
        }

        var user = await _userRepository.GetUserByUserNameAsync(userName);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        bool isCustomer = await _userRepository.IsInRoleAsync(user, UserRoles.Customer);
        bool isAdmin = await _userRepository.IsInRoleAsync(user, UserRoles.Admin);
        if (!(isCustomer || isAdmin))
        {
            result.IsValid = false;
            result.Error = "The authorization is denied.";
            return result;
        }

        var isValidPassword = await _userRepository.CheckUserPasswordAsync(user, password);
        if (!isValidPassword)
        {
            result.IsValid = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        return result;
    }

    public ValidationResult ValidateRefreshTokenModel(string? securityToken, string? refreshToken)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (string.IsNullOrEmpty(securityToken) || string.IsNullOrEmpty(refreshToken))
        {
            result.IsValid = false;
            result.Error = "The passed data is null.";
            return result;
        }

        return result;
    }

    public ValidationResult ValidateExpiredJwt(JwtSecurityToken? jwt, bool isAlgorithmValid)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (jwt is null)
        {
            result.IsValid = false;
            result.Error = "The passed JWT is null.";
            return result;
        }

        if (!isAlgorithmValid)
        {
            result.IsValid = false;
            result.Error = "The token is not valid";
            return result;
        }
        else if (jwt.ValidTo > DateTime.UtcNow)
        {
            result.IsValid = false;
            result.Error = "The token has not expired yet";
            return result;
        }

        return result;
    }

    public ValidationResult ValidateRefreshToken(RefreshToken? model, string? jwtId)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed refresh token is null";
            return result;
        }
        else if (DateTime.UtcNow > model.ExpiryDate)
        {
            result.IsValid = false;
            result.Error = "The refresh token has expired";
            return result;
        }
        else if (model.Invalidated)
        {
            result.IsValid = false;
            result.Error = "The refresh token has been invalidated";
            return result;
        }
        else if (model.Used)
        {
            result.IsValid = false;
            result.Error = "The refresh token has been used";
            return result;
        }
        else if (model.JwtId != jwtId)
        {
            result.IsValid = false;
            result.Error = "The refresh token does not match the JWT";
            return result;
        }

        return result;
    }
}
