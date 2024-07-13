using DokWokApi.BLL;
using DokWokApi.BLL.Models.User;
using DokWokApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.Validation;

public class UserServiceValidator : IUserServiceValidator
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserServiceValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ValidationResult> ValidateAddAsync(UserModel model)
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

        bool isPhoneNumberTaken = await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
        if (isPhoneNumberTaken)
        {
            result.IsValid = false;
            result.Error = "The phone number is already taken";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(UserModel model)
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

        var user = await _userManager.FindByIdAsync(model.Id!);
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

        if (model.PhoneNumber != user.PhoneNumber && await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
        {
            result.IsValid = false;
            result.Error = "The phone number is already taken";
            return result;
        }

        return result;
    }

    public async Task<ValidationResult> ValidateAdminLoginAsync(UserLoginModel model)
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

    public async Task<ValidationResult> ValidateCustomerLoginAsync(UserLoginModel model)
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

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel model)
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

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(UserPasswordChangeModel model)
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
}
