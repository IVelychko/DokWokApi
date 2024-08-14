using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.Exceptions;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly StoreDbContext _context;
    private readonly IRefreshTokenRepositoryValidator _validator;

    public RefreshTokenRepository(StoreDbContext context, IRefreshTokenRepositoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<RefreshToken>> AddAsync(RefreshToken entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.ToDictionary());
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

    public async Task<IEnumerable<RefreshToken>> GetAllAsync()
    {
        return await _context.RefreshTokens.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.RefreshTokens
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllWithDetailsAsync()
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
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
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
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
