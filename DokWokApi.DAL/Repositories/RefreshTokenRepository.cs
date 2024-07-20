using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly StoreDbContext _context;
    private readonly IValidator<RefreshToken> _validator;

    public RefreshTokenRepository(StoreDbContext context, IValidator<RefreshToken> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<RefreshToken>> AddAsync(RefreshToken entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<RefreshToken>(exception);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : new Result<RefreshToken>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<RefreshToken>(exception);
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

    public IQueryable<RefreshToken> GetAll()
    {
        return _context.RefreshTokens.AsNoTracking();
    }

    public IQueryable<RefreshToken> GetAllWithDetails()
    {
        return _context.RefreshTokens.Include(rt => rt.User).AsNoTracking();
    }

    public async Task<RefreshToken?> GetByIdAsync(long id)
    {
        return await _context.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.RefreshTokens.Include(rt => rt.User).AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
    {
        return await _context.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByJwtIdWithDetailsAsync(string jwtId)
    {
        return await _context.RefreshTokens.Include(rt => rt.User).AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByTokenWithDetailsAsync(string token)
    {
        return await _context.RefreshTokens.Include(rt => rt.User).AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetByUserIdAsync(string userId)
    {
        return await _context.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.UserId == userId);
    }

    public async Task<RefreshToken?> GetByUserIdWithDetailsAsync(string userId)
    {
        return await _context.RefreshTokens.Include(rt => rt.User).AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.UserId == userId);
    }

    public async Task<Result<RefreshToken>> UpdateAsync(RefreshToken entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<RefreshToken>(exception);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : new Result<RefreshToken>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<RefreshToken>(exception);
        }
    }
}
