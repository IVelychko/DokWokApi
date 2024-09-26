using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Models;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly StoreDbContext _context;

    public ShopRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Shop>> AddAsync(Shop entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("shop", "The passed entity is null");
            return Result<Shop>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdAsync(entity.Id);
            return addedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<Shop>(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Shop>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var query = _context.Shops.AsNoTracking();
        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<Shop?> GetByIdAsync(long id)
    {
        return await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shop?> GetByAddressAsync(string street, string building)
    {
        return await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
    }

    public async Task<Result<Shop>> UpdateAsync(Shop entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("shop", "The passed entity is null");
            return Result<Shop>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdAsync(entity.Id);
            return updatedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<Result<bool>> IsAddressTakenAsync(string street, string building)
    {
        if (string.IsNullOrEmpty(street))
        {
            var error = new ValidationError(nameof(street), "The passed street is null or empty");
            return Result<bool>.Failure(error);
        }
        else if (string.IsNullOrEmpty(building))
        {
            var error = new ValidationError(nameof(building), "The passed building is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.Shops.AnyAsync(s => s.Street == street && s.Building == building);
        return isTaken;
    }
}
