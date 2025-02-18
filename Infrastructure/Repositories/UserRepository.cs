using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

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

    public async Task AddAsync(User entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public async Task<bool> CheckUserPasswordAsync(long userId, string password)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(password);
        User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
        return user is not null && _passwordHasher.Verify(password, user.PasswordHash);
    }

    public bool CheckUserPassword(User user, string password)
    {
        Ensure.ArgumentNotNull(user);
        Ensure.ArgumentNotNullOrWhiteSpace(password);
        return _passwordHasher.Verify(password, user.PasswordHash);
    }

    public void Delete(User entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<User>> GetAllUsersBySpecificationAsync(Specification<User> specification)
    {
        IQueryable<User> query = SpecificationEvaluator.ApplySpecification(_context.Users, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<User>> GetAllCustomersBySpecificationAsync(Specification<User> specification)
    {
        IQueryable<User> query = SpecificationEvaluator
            .ApplySpecification(_context.Users.Where(u => u.UserRole!.Name == UserRoles.Customer),
            specification);
        return await query.ToListAsync();
    }

    public async Task<IList<User>> GetAllCustomersAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users.Where(u => u.UserRole!.Name == UserRoles.Customer);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<User>> GetAllCustomersWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users
            .Include(u => u.UserRole)
            .Where(u => u.UserRole!.Name == UserRoles.Customer);

        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<User>> GetAllUsersAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<User>> GetAllUsersWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<User> query = _context.Users.Include(u => u.UserRole);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
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
        return await _context.FindAsync<User>(id);
    }

    public async Task<User?> GetUserByIdWithDetailsAsync(long id)
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<User?> GetUserByIdWithDetailsAsNoTrackingAsync(long id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
    
    public async Task<User?> GetUserByUserNameWithDetailsAsNoTrackingAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetUserByUserNameWithDetailsAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        return await _context.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email);
        bool isTaken = await _context.Users.AnyAsync(u => u.Email == email);
        return !isTaken;
    }
    
    public async Task<bool> IsEmailUniqueAsync(string email, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email);
        bool isTaken = await _context.Users
            .AnyAsync(u => u.Email == email && u.Id != idToExclude);
        return !isTaken;
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber);
        bool isTaken = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        return !isTaken;
    }
    
    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber);
        bool isTaken = await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != idToExclude);
        return !isTaken;
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        bool isTaken = await _context.Users.AnyAsync(u => u.UserName == userName);
        return !isTaken;
    }
    
    public async Task<bool> IsUserNameUniqueAsync(string userName, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName);
        bool isTaken = await _context.Users
            .AnyAsync(u => u.UserName == userName && u.Id != idToExclude);
        return !isTaken;
    }

    public void Update(User entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
    
    public async Task<bool> UserExistsAsync(long id)
    {
        return await _context.Users.AnyAsync(x => x.Id == id);
    }
    
    public async Task<bool> CustomerExistsAsync(long id)
    {
        return await _context.Users.AnyAsync(x => x.Id == id && x.UserRole!.Name == UserRoles.Customer);
    }
}
