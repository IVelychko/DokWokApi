using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Helpers;
using Domain.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation;

public class UserRepositoryValidator : IUserRepositoryValidator
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepositoryValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ValidationResult> ValidateAddAsync(ApplicationUser? model)
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

    public async Task<ValidationResult> ValidateUpdateAsync(ApplicationUser? model)
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


        if (model.PhoneNumber != user.PhoneNumber)
        {
            bool isPhoneNumberTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (isPhoneNumberTaken)
            {
                result.IsValid = false;
                result.Error = "The phone number is already taken";
                return result;
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(string? userId, string? newPassword)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
        {
            result.IsValid = false;
            result.Error = "The passed data is null.";
            return result;
        }

        var user = await _userManager.FindByIdAsync(userId);
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

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(string? userId, string? oldPassword, string? newPassword)
    {
        ValidationResult result = new()
        {
            IsValid = true,
            IsFound = true,
        };
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            result.IsValid = false;
            result.Error = "The passed data is null.";
            return result;
        }

        var user = await _userManager.FindByIdAsync(userId);
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

        bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, oldPassword);
        if (!isOldPasswordValid)
        {
            result.IsValid = false;
            result.Error = "The old password is not valid.";
            return result;
        }

        return result;
    }
}
