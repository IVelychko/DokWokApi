using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext context;

    public ProductRepository(StoreDbContext context)
    {
        this.context = context;
    }

    public async Task<Product> AddAsync(Product entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var category = await context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.CategoryId);
        RepositoryHelper.CheckRetrievedEntity(category, "There is no product category with the ID specified in the CategoryId property of the Product entity.");
        RepositoryHelper.ThrowIfExists(await context.Products.AnyAsync(p => p.Name == entity.Name),
            "The entity with the same Name value is already present in the database.");
        
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Product entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await context.FindAsync<Product>(entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToDelete, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await context.FindAsync<Product>(id);
        entity = RepositoryHelper.CheckRetrievedEntity(entity, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<Product> GetAll()
    {
        return context.Products;
    }

    public IQueryable<Product> GetAllWithDetails()
    {
        return context.Products.Include(p => p.Category);
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await context.FindAsync<Product>(id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(long id)
    {
        return await context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.Id);
        entityToUpdate = RepositoryHelper.CheckRetrievedEntity(entityToUpdate, "There is no entity with this ID in the database.");
        var category = await context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.CategoryId);
        RepositoryHelper.CheckRetrievedEntity(category, "There is no product category with the ID specified in the CategoryId property of the Product entity.");
        if (entity.Name != entityToUpdate.Name)
        {
            RepositoryHelper.ThrowIfExists(await context.Products.AnyAsync(p => p.Name == entity.Name),
            "The entity with the same Name value is already present in the database.");
        }

        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
