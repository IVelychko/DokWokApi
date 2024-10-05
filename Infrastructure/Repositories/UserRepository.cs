using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
using Domain.Models;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StoreDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(StoreDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Unit>> AddAsync(User entity, string password)
    {
        if (entity is null)
        {
            var error = new ValidationError("user", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }
        else if (string.IsNullOrEmpty(password))
        {
            var error = new ValidationError(nameof(password), "The passed password is null or empty");
            return Result<Unit>.Failure(error);
        }

        var hashedPassword = _passwordHasher.Hash(password);
        entity.PasswordHash = hashedPassword;
        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task<bool> CheckUserPasswordAsync(long userId, string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        var user = await GetUserByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        return _passwordHasher.Verify(password, user.PasswordHash);
    }

    public bool CheckUserPassword(User user, string password)
    {
        if (user is null || string.IsNullOrEmpty(password))
        {
            return false;
        }

        return _passwordHasher.Verify(password, user.PasswordHash);
    }

    public async Task DeleteByIdAsync(long id)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id && u.UserRole!.Name == UserRoles.Customer);
        _context.Remove(user);
    }

    public async Task<IEnumerable<User>> GetAllUsersBySpecificationAsync(Specification<User> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.Users, specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllCustomersBySpecificationAsync(Specification<User> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.Users.Where(u => u.UserRole!.Name == UserRoles.Customer),
            specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllCustomersAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users.Where(u => u.UserRole!.Name == UserRoles.Customer);
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllCustomersWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users
            .Include(u => u.UserRole)
            .Where(u => u.UserRole!.Name == UserRoles.Customer);

        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users;
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllUsersWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users.Include(u => u.UserRole);
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<User?> GetCustomerByIdAsync(long id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.UserRole!.Name == UserRoles.Customer);
    }

    public async Task<User?> GetCustomerByIdWithDetailsAsync(long id)
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Id == id && u.UserRole!.Name == UserRoles.Customer);
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByIdWithDetailsAsync(long id)
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetUserByUserNameWithDetailsAsync(string userName)
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<Result<bool>> IsEmailTakenAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            var error = new ValidationError(nameof(email), "The passed email is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.Users.AnyAsync(u => u.Email == email);
        return isTaken;
    }

    public async Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            var error = new ValidationError(nameof(phoneNumber), "The passed phone number is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        return isTaken;
    }

    public async Task<Result<bool>> IsUserNameTakenAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            var error = new ValidationError(nameof(userName), "The passed user name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.Users.AnyAsync(u => u.UserName == userName);
        return isTaken;
    }

    public Result<Unit> Update(User entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("user", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
    }

    public async Task<Result<Unit>> UpdateCustomerPasswordAsAdminAsync(long userId, string newPassword)
    {
        if (string.IsNullOrEmpty(newPassword))
        {
            var error = new ValidationError("userData", "The passed userId or newPassword is null");
            return Result<Unit>.Failure(error);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            var error = new EntityNotFoundError(nameof(user), "The user was not found");
            return Result<Unit>.Failure(error);
        }

        var hashedPassword = _passwordHasher.Hash(newPassword);
        user.PasswordHash = hashedPassword;
        var result = Update(user);
        return result;
    }

    public async Task<Result<Unit>> UpdateCustomerPasswordAsync(long userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            var error = new ValidationError("userData", "The passed userId, oldPassword or newPassword is null");
            return Result<Unit>.Failure(error);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            var error = new EntityNotFoundError(nameof(user), "The user was not found");
            return Result<Unit>.Failure(error);
        }

        var isOldPasswordValid = CheckUserPassword(user, oldPassword);
        if (!isOldPasswordValid)
        {
            var error = new ValidationError("userData", "The passed credentials are not valid");
            return Result<Unit>.Failure(error);
        }

        var hashedPassword = _passwordHasher.Hash(newPassword);
        user.PasswordHash = hashedPassword;
        var result = Update(user);
        return result;
    }
}
