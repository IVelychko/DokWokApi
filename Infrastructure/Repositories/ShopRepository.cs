using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Specification;
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

    public async Task<IList<Shop>> GetAllBySpecificationAsync(Specification<Shop> specification)
    {
        IQueryable<Shop> query = SpecificationEvaluator.ApplySpecification(_context.Shops, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<Shop>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Shop> query = _context.Shops;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<Shop?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Shop>(id);
    }

    public async Task<Shop?> GetByAddressAsync(string street, string building)
    {
        return await _context.Shops.FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
    }

    public void Update(Shop entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }

    public async Task<bool> IsAddressUniqueAsync(string street, string building)
    {
        Ensure.ArgumentNotNull(street);
        Ensure.ArgumentNotNull(building);
        bool isTaken = await _context.Shops.AsNoTracking()
            .AnyAsync(s => s.Street == street && s.Building == building);
        return !isTaken;
    }
}
