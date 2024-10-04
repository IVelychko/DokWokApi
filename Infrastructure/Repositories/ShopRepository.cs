using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
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

    public async Task<Result<Unit>> AddAsync(Shop entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("shop", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.Shops.FirstAsync(s => s.Id == id);
        _context.Remove(entity);
    }

    public async Task<IEnumerable<Shop>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Shop> query = _context.Shops;
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
        return await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shop?> GetByAddressAsync(string street, string building)
    {
        return await _context.Shops.FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
    }

    public Result<Unit> Update(Shop entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("shop", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
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
