using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext context;

    public ProductCategoryRepository(StoreDbContext context)
    {
        this.context = context;
    }

    public async Task<ProductCategory> AddAsync(ProductCategory entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        RepositoryHelper.ThrowIfExists(await context.ProductCategories.AnyAsync(c => c.Name == entity.Name),
            "The entity with the same Name value is already present in the database.");

        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProductCategory entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await context.FindAsync<ProductCategory>(entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToDelete, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await context.FindAsync<ProductCategory>(id);
        RepositoryHelper.CheckRetrievedEntity(entity, "There is no entity with this ID in the database.");

        context.Remove(entity!);
        await context.SaveChangesAsync();
    }

    public IQueryable<ProductCategory> GetAll()
    {
        return context.ProductCategories;
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await context.FindAsync<ProductCategory>(id);
    }

    public async Task<ProductCategory> UpdateAsync(ProductCategory entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToUpdate, "There is no entity with this ID in the database.");
        if (entity.Name != entityToUpdate!.Name)
        {
            RepositoryHelper.ThrowIfExists(await context.ProductCategories.AnyAsync(c => c.Name == entity.Name),
                "The entity with the same Name value is already present in the database.");
        }

        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
