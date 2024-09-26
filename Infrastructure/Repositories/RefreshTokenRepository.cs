using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Models;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly StoreDbContext _context;

    public RefreshTokenRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RefreshToken>> AddAsync(RefreshToken entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("refreshToken", "The passed entity is null");
            return Result<RefreshToken>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<RefreshToken>(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<RefreshToken>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var query = _context.RefreshTokens.AsNoTracking();
        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<RefreshToken> query = _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<RefreshToken?> GetByIdAsync(long id)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByJwtIdWithDetailsAsync(string jwtId)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByTokenWithDetailsAsync(string token)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByUserIdAsync(string userId)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.UserId == userId);
    }

    public async Task<RefreshToken?> GetByUserIdWithDetailsAsync(string userId)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == userId);
    }

    public async Task<Result<RefreshToken>> UpdateAsync(RefreshToken entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("refreshToken", "The passed entity is null");
            return Result<RefreshToken>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }
}
