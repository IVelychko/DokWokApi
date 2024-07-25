using DokWokApi.BLL.Infrastructure;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace DokWokApi.BLL.Validation;

public class UserServiceValidator : IUserServiceValidator
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserServiceValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ValidationResult> ValidateAddAsync(UserModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
            return result;
        }

        if (model.PhoneNumber is null)
        {
            result.IsValid = false;
            result.Error = "The phone number is null";
            return result;
        }

        bool isPhoneNumberTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
        if (isPhoneNumberTaken)
        {
            result.IsValid = false;
            result.Error = "The phone number is already taken";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(UserModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
            return result;
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with this ID in the database.";
            return result;
        }

        if (model.PhoneNumber is null)
        {
            result.IsValid = false;
            result.Error = "The phone number is null";
            return result;
        }

        if (model.PhoneNumber != user.PhoneNumber && await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
        {
            result.IsValid = false;
            result.Error = "The phone number is already taken";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateAdminLoginAsync(UserLoginModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
            return result;
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (!isAdmin)
        {
            result.IsValid = false;
            result.Error = "The authorization is denied.";
            return result;
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isValidPassword)
        {
            result.IsValid = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateCustomerLoginAsync(UserLoginModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
            return result;
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        bool isCustomer = await _userManager.IsInRoleAsync(user, UserRoles.Customer);
        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (!(isCustomer || isAdmin))
        {
            result.IsValid = false;
            result.Error = "The authorization is denied.";
            return result;
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isValidPassword)
        {
            result.IsValid = false;
            result.Error = "The credentials are wrong.";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
            return result;
        }

        var user = await _userManager.FindByIdAsync(model.UserId!);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with this ID in the database.";
            return result;
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdmin)
        {
            result.IsValid = false;
            result.Error = "Forbidden action";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(UserPasswordChangeModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
            return result;
        }

        var user = await _userManager.FindByIdAsync(model.UserId!);
        if (user is null)
        {
            result.IsValid = false;
            result.IsFound = false;
            result.Error = "There is no user with this ID in the database.";
            return result;
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdmin)
        {
            result.IsValid = false;
            result.Error = "Forbidden action";
            return result;
        }

        bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword!);
        if (!isOldPasswordValid)
        {
            result.IsValid = false;
            result.Error = "The old password is not valid.";
            return result;
        }

        return result;
    }

    public ValidationResult ValidateRefreshTokenModel(RefreshTokenModel? model)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (model is null)
        {
            result.IsValid = false;
            result.Error = "The passed user is null.";
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

    public ValidationResult ValidateRefreshToken(RefreshToken? model, string jwtId)
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
