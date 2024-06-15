using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly StoreDbContext _context;

    public ShopRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Shop> AddAsync(Shop entity)
    {
        RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "The passed entity is null.");
        var isAddressTaken = await _context.Shops.AnyAsync(s => s.Street == entity.Street && s.Building == entity.Building);
        RepositoryHelper.ThrowArgumentExceptionIfTrue(isAddressTaken, "The entity with the same Street and Building values is already present in the database.");

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Shop entity)
    {
        RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "The passed entity is null.");
        var entityToDelete = await _context.FindAsync<Shop>(entity.Id);
        RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(entityToDelete, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<Shop>(id);
        entity = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(entity, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<Shop> GetAll()
    {
        return _context.Shops.AsNoTracking();
    }

    public async Task<Shop?> GetByIdAsync(long id)
    {
        return await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shop?> GetByAddressAsync(string street, string building)
    {
        return await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
    }

    public async Task<Shop> UpdateAsync(Shop entity)
    {
        RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "The passed entity is null.");
        var entityToUpdate = await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Id == entity.Id);
        entityToUpdate = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(entityToUpdate, "There is no entity with this ID in the database.");
        if (entity.Street != entityToUpdate.Street || entity.Building != entityToUpdate.Building)
        {
            var isAddressTaken = await _context.Shops.AnyAsync(s => s.Street == entity.Street && s.Building == entity.Building);
            RepositoryHelper.ThrowArgumentExceptionIfTrue(isAddressTaken,
                "The entity with the same Street and Building values is already present in the database.");
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
