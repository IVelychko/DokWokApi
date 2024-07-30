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
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed user is null");
            return result;
        }

        if (string.IsNullOrEmpty(model.PhoneNumber))
        {
            result.IsValid = false;
            result.Errors.Add("The phone number is null or empty");
            return result;
        }

        bool isPhoneNumberTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
        if (isPhoneNumberTaken)
        {
            result.IsValid = false;
            result.Errors.Add("The phone number is already taken");
        }

        return result;
    }

    public ValidationResult ValidateCheckPassword(ApplicationUser model, string password)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed model is null");
        }

        if (string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Errors.Add("The passed password is null or empty");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(ApplicationUser? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed user is null");
            return result;
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no user with this ID in the database");
        }

        if (string.IsNullOrEmpty(model.PhoneNumber))
        {
            result.IsValid = false;
            result.Errors.Add("The phone number is null or empty");
        }


        if (user is not null && !string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber != user.PhoneNumber)
        {
            bool isPhoneNumberTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (isPhoneNumberTaken)
            {
                result.IsValid = false;
                result.Errors.Add("The phone number is already taken");
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(string? userId, string? newPassword)
    {
        ValidationResult result = new(true);
        if (string.IsNullOrEmpty(userId))
        {
            result.IsValid = false;
            result.Errors.Add("The passed user id is null or empty");
            return result;
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            result.IsValid = false;
            result.Errors.Add("The passed new password is null or empty");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no user with this ID in the database");
            return result;
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdmin)
        {
            result.IsValid = false;
            result.Errors.Add("Forbidden action");
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(string? userId, string? oldPassword, string? newPassword)
    {
        ValidationResult result = new(true);
        if (string.IsNullOrEmpty(userId))
        {
            result.IsValid = false;
            result.Errors.Add("The passed user id is null or empty");
            return result;
        }

        if (string.IsNullOrEmpty(oldPassword))
        {
            result.IsValid = false;
            result.Errors.Add("The passed old password is null or empty");
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            result.IsValid = false;
            result.Errors.Add("The passed new password is null or empty");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no user with this ID in the database");
            return result;
        }

        bool isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdmin)
        {
            result.IsValid = false;
            result.Errors.Add("Forbidden action");
            return result;
        }

        if (oldPassword is not null)
        {
            bool isOldPasswordValid = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!isOldPasswordValid)
            {
                result.IsValid = false;
                result.Errors.Add("The old password is not valid");
            }
        }

        return result;
    }
}
