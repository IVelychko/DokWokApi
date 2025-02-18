using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly StoreDbContext _context;

    public RefreshTokenRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(RefreshToken entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<RefreshToken>> GetAllBySpecificationAsync(Specification<RefreshToken> specification)
    {
        IQueryable<RefreshToken> query = SpecificationEvaluator
            .ApplySpecification(_context.RefreshTokens, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<RefreshToken>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<RefreshToken> query = _context.RefreshTokens;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<RefreshToken>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<RefreshToken> query = _context.RefreshTokens.Include(rt => rt.User);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<RefreshToken?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<RefreshToken>(id);
    }

    public async Task<RefreshToken?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(jwtId);
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByJwtIdWithDetailsAsync(string jwtId)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(jwtId);
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token);
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }
    
    public async Task<RefreshToken?> GetByTokenAsNoTrackingAsync(string token)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token);
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByTokenWithDetailsAsync(string token)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(token);
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByUserIdAsync(long userId)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
    }

    public async Task<RefreshToken?> GetByUserIdWithDetailsAsync(long userId)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == userId);
    }

    public void Update(RefreshToken entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
    
    public async Task<bool> RefreshTokenExistsAsync(long id)
    {
        var exists = await _context.RefreshTokens
            .AnyAsync(x => x.Id == id);
        return exists;
    }
}
