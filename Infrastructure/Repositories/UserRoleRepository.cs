using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Helpers;
using Domain.Models;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly StoreDbContext _context;

    public UserRoleRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserRole>> GetAllBySpecificationAsync(Specification<UserRole> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.UserRoles, specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<UserRole> query = _context.UserRoles;
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<UserRole?> GetByIdAsync(long id)
    {
        return await _context.UserRoles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<UserRole?> GetByNameAsync(string name)
    {
        return await _context.UserRoles.FirstOrDefaultAsync(r => r.Name == name);
    }
}
