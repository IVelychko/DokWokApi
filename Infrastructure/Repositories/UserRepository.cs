using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Models;
using Domain.ResultType;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<ApplicationUser>> AddAsync(ApplicationUser entity, string password)
    {
        if (entity is null)
        {
            var error = new ValidationError("user", "The passed entity is null");
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

    public async Task<IEnumerable<ApplicationUser>> GetAllCustomersAsync(PageInfo? pageInfo = null)
    {
        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            var customers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
            return customers.Skip(itemsToSkip).Take(pageInfo.PageSize);
        }

        return await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(PageInfo? pageInfo = null)
    {
        var query = _userManager.Users.AsNoTracking();
        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
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
        if (entity is null)
        {
            var error = new ValidationError("user", "The passed entity is null");
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
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
        {
            var error = new ValidationError("userData", "The passed userId or newPassword is null");
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
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            var error = new ValidationError("userData", "The passed userId, oldPassword or newPassword is null");
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
