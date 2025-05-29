using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly StoreDbContext _context;

    public UserRoleRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IList<UserRole>> GetAllAsync()
    {
        return await _context.UserRoles.ToListAsync();
    }

    public async Task<UserRole?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<UserRole>(id);
    }

    public async Task<UserRole?> GetByNameAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        return await _context.UserRoles.FirstOrDefaultAsync(r => r.Name == name);
    }
    
    public async Task<bool> UserRoleExistsAsync(long id)
    {
        return await _context.UserRoles.AnyAsync(x => x.Id == id);
    }
}
