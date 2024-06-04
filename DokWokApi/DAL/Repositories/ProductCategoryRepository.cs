using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext _context;

    public ProductCategoryRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<ProductCategory> AddAsync(ProductCategory entity)
    {
        RepositoryHelper.ThrowIfNull(entity, "The passed entity is null.");
        RepositoryHelper.ThrowIfTrue(await _context.ProductCategories.AnyAsync(c => c.Name == entity.Name),
            "The entity with the same Name value is already present in the database.");

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProductCategory entity)
    {
        RepositoryHelper.ThrowIfNull(entity, "The passed entity is null.");
        var entityToDelete = await _context.FindAsync<ProductCategory>(entity.Id);
        RepositoryHelper.ThrowEntityNotFoundIfNull(entityToDelete, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<ProductCategory>(id);
        entity = RepositoryHelper.ThrowEntityNotFoundIfNull(entity, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<ProductCategory> GetAll()
    {
        return _context.ProductCategories.AsNoTracking();
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ProductCategory> UpdateAsync(ProductCategory entity)
    {
        RepositoryHelper.ThrowIfNull(entity, "The passed entity is null.");
        var entityToUpdate = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.Id);
        entityToUpdate = RepositoryHelper.ThrowEntityNotFoundIfNull(entityToUpdate, "There is no entity with this ID in the database.");
        if (entity.Name != entityToUpdate.Name)
        {
            RepositoryHelper.ThrowIfTrue(await _context.ProductCategories.AnyAsync(c => c.Name == entity.Name),
                "The entity with the same Name value is already present in the database.");
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
