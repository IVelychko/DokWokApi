using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly StoreDbContext _context;

    public ShopRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Shop entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(Shop entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }
    
    public async Task<IList<Shop>> GetAllAsync()
    {
        return await _context.Shops.ToListAsync();
    }

    public async Task<Shop?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Shop>(id);
    }

    public async Task<Shop?> GetByAddressAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        return await _context.Shops.FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
    }

    public void Update(Shop entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }

    public async Task<bool> IsAddressUniqueAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        var isTaken = await _context.Shops
            .AnyAsync(s => s.Street == street && s.Building == building);
        return !isTaken;
    }
    
    public async Task<bool> IsAddressUniqueAsync(string street, string building, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        var isTaken = await _context.Shops
            .AnyAsync(s => s.Street == street && s.Building == building && s.Id != idToExclude);
        return !isTaken;
    }
    
    public async Task<bool> ShopExistsAsync(long id)
    {
        var exists = await _context.Shops.AnyAsync(x => x.Id == id);
        return exists;
    }
}
