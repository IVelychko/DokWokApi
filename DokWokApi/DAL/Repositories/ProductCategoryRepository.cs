using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext context;

    public ProductCategoryRepository(StoreDbContext context)
    {
        this.context = context;
    }

    private static void CheckForNull(ProductCategory? entity, string errorMessage)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), errorMessage);
        }
    }

    private static void CheckRetrievedCategory(ProductCategory? entity, string errorMessage)
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(entity), errorMessage);
        }
    }

    public async Task<ProductCategory> AddAsync(ProductCategory entity)
    {
        CheckForNull(entity, "The passed entity is null.");

        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProductCategory entity)
    {
        CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await context.FindAsync<ProductCategory>(entity.Id);
        CheckRetrievedCategory(entityToDelete, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await context.FindAsync<ProductCategory>(id);
        CheckRetrievedCategory(entity, "There is no entity with this ID in the database.");

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
        CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.Id);
        CheckRetrievedCategory(entityToUpdate, "There is no entity with this ID in the database.");

        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
