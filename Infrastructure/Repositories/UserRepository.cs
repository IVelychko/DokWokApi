using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.ResultType;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepositoryValidator _validator;

    public UserRepository(UserManager<ApplicationUser> userManager, IUserRepositoryValidator validator)
    {
        _userManager = userManager;
        _validator = validator;
    }

    public async Task<Result<ApplicationUser>> AddAsync(ApplicationUser entity, string password)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.ToDictionary());
            return Result<ApplicationUser>.Failure(error);
        }
        else if (string.IsNullOrEmpty(password))
        {
            var error = new ValidationError(nameof(password), "The passed password is null or empty");
            return Result<ApplicationUser>.Failure(error);
        }

        var result = await _userManager.CreateAsync(entity, password);
        if (!result.Succeeded)
        {
            Dictionary<string, string[]> errors = new() { [nameof(result)] = result.Errors.Select(e => e.Description).ToArray() };
            var error = new ValidationError(errors);
            return Result<ApplicationUser>.Failure(error);
        }

        var addedUser = await _userManager.FindByIdAsync(entity.Id);
        if (addedUser is not null)
        {
            await _userManager.AddToRoleAsync(addedUser, UserRoles.Customer);
            return addedUser;
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<Result<bool>> AddToRoleAsync(ApplicationUser entity, string role)
    {
        if (entity is null || string.IsNullOrEmpty(role))
        {
            Dictionary<string, string[]> errors = [];
            if (entity is null)
            {
                errors.Add(nameof(entity), ["The passed entity is null"]);
            }

            if (string.IsNullOrEmpty(role))
            {
                errors.Add(nameof(role), ["The passed role is null or empty"]);
            }

            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var result = await _userManager.AddToRoleAsync(entity, role);
        if (!result.Succeeded)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> CheckUserPasswordAsync(ApplicationUser entity, string password)
    {
        if (entity is null || string.IsNullOrEmpty(password))
        {
            return false;
        }

        var result = await _userManager.CheckPasswordAsync(entity, password);
        return result;
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
        if (isAdmin)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllCustomersAsync()
    {
        return await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllCustomersByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        var customers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
        return customers.Skip(itemsToSkip).Take(pageSize);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _userManager.Users
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ApplicationUser?> GetCustomerByIdAsync(string id)
    {
        var entity = await _userManager.FindByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var isCustomer = await _userManager.IsInRoleAsync(entity, UserRoles.Customer);
        return isCustomer ? entity : null;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            var error = new EntityNotFoundError(nameof(user), "There is no user with this id.");
            return Result<IEnumerable<string>>.Failure(error);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new Result<IEnumerable<string>>(roles);
    }

    public async Task<Result<bool>> IsEmailTakenAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            var error = new ValidationError(nameof(email), "The passed email is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.Email == email);
        return isTaken;
    }

    public async Task<Result<bool>> IsInRoleAsync(ApplicationUser entity, string role)
    {
        if (entity is null || string.IsNullOrEmpty(role))
        {
            Dictionary<string, string[]> errors = [];
            if (entity is null)
            {
                errors.Add(nameof(entity), ["The passed entity is null"]);
            }

            if (string.IsNullOrEmpty(role))
            {
                errors.Add(nameof(role), ["The passed role is null or empty"]);
            }

            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var result = await _userManager.IsInRoleAsync(entity, role);
        return result;
    }

    public async Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            var error = new ValidationError(nameof(phoneNumber), "The passed phone number is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == phoneNumber);
        return isTaken;
    }

    public async Task<Result<bool>> IsUserNameTakenAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            var error = new ValidationError(nameof(userName), "The passed user name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _userManager.Users.AsNoTracking().AnyAsync(u => u.UserName == userName);
        return isTaken;
    }

    public async Task<Result<ApplicationUser>> UpdateAsync(ApplicationUser entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
            return Result<ApplicationUser>.Failure(error);
        }

        var result = await _userManager.UpdateAsync(entity);
        if (!result.Succeeded)
        {
            Dictionary<string, string[]> errors = new() { [nameof(result)] = result.Errors.Select(e => e.Description).ToArray() };
            var error = new ValidationError(errors);
            return Result<ApplicationUser>.Failure(error);
        }

        var updatedUser = await _userManager.FindByIdAsync(entity.Id);
        if (updatedUser is not null)
        {
            return updatedUser;
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(string userId, string newPassword)
    {
        var validationResult = await _validator.ValidateUpdateCustomerPasswordAsAdminAsync(userId, newPassword);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
            return Result<bool>.Failure(error);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            var error = new EntityNotFoundError(nameof(user), "The user was not found");
            return Result<bool>.Failure(error);
        }

        var oldPasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!oldPasswordResult.Succeeded)
        {
            Dictionary<string, string[]> errors = new() { [nameof(oldPasswordResult)] = oldPasswordResult.Errors.Select(e => e.Description).ToArray() };
            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var newPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (!newPasswordResult.Succeeded)
        {
            Dictionary<string, string[]> errors = new() { [nameof(oldPasswordResult)] = newPasswordResult.Errors.Select(e => e.Description).ToArray() };
            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        return true;
    }

    public async Task<Result<bool>> UpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var validationResult = await _validator.ValidateUpdateCustomerPasswordAsync(userId, oldPassword, newPassword);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
            return Result<bool>.Failure(error);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            var error = new EntityNotFoundError(nameof(user), "The user was not found");
            return Result<bool>.Failure(error);
        }

        var oldPasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!oldPasswordResult.Succeeded)
        {
            Dictionary<string, string[]> errors = new() { [nameof(oldPasswordResult)] = oldPasswordResult.Errors.Select(e => e.Description).ToArray() };
            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var newPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (!newPasswordResult.Succeeded)
        {
            Dictionary<string, string[]> errors = new() { [nameof(oldPasswordResult)] = newPasswordResult.Errors.Select(e => e.Description).ToArray() };
            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        return true;
    }
}
