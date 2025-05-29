using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.Users;
using Infrastructure.Specifications.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StoreDbContext _context;

    public UserRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(User entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    
    public async Task<IList<User>> GetAllBySpecificationAsync(UserSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = UserSpecificationEvaluator.ApplySpecification(_context.Users, specification);
        return await query.ToListAsync();
    }
    
    public async Task<IList<User>> GetAllByRoleIdAsync(long roleId)
    {
        return await _context.Users
            .Where(u => u.UserRoleId == roleId)
            .ToListAsync();
    }
    
    public async Task<User?> GetBySpecificationAsync(UserSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = UserSpecificationEvaluator.ApplySpecification(_context.Users, specification);
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<User?> GetByUserNameAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email, nameof(email));
        var isTaken = await _context.Users.AnyAsync(u => u.Email == email);
        return !isTaken;
    }
    
    public async Task<bool> IsEmailUniqueAsync(string email, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(email, nameof(email));
        var isTaken = await _context.Users
            .AnyAsync(u => u.Email == email && u.Id != idToExclude);
        return !isTaken;
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        var isTaken = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        return !isTaken;
    }
    
    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        var isTaken = await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != idToExclude);
        return !isTaken;
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        var isTaken = await _context.Users.AnyAsync(u => u.UserName == userName);
        return !isTaken;
    }
    
    public async Task<bool> IsUserNameUniqueAsync(string userName, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(userName, nameof(userName));
        var isTaken = await _context.Users
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
    
    public async Task<bool> UserExistsAsync(long userId, long roleIdToExclude)
    {
        return await _context.Users.AnyAsync(x => x.Id == userId && x.UserRoleId != roleIdToExclude);
    }
}
