using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
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

    public async Task<Result<Unit>> AddAsync(RefreshToken entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("refreshToken", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.RefreshTokens.FirstAsync(rt => rt.Id == id);
        _context.Remove(entity);
    }

    public async Task<IEnumerable<RefreshToken>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<RefreshToken> query = _context.RefreshTokens;
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
        IQueryable<RefreshToken> query = _context.RefreshTokens.Include(rt => rt.User);

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
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByJwtIdWithDetailsAsync(string jwtId)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByTokenWithDetailsAsync(string token)
    {
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

    public Result<Unit> Update(RefreshToken entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("refreshToken", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
    }
}
