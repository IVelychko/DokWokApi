using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext context;

    public ProductRepository(StoreDbContext context)
    {
        this.context = context;
    }

    private static void CheckForNull(Product? entity, string errorMessage)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), errorMessage);
        }
    }

    private static void CheckRetrievedProduct(Product? entity, string errorMessage)
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(entity), errorMessage);
        }
    }

    public async Task<Product> AddAsync(Product entity)
    {
        CheckForNull(entity, "The passed entity is null.");

        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Product entity)
    {
        CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await context.FindAsync<Product>(entity.Id);
        CheckRetrievedProduct(entityToDelete, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await context.FindAsync<Product>(id);
        CheckRetrievedProduct(entity, "There is no entity with this ID in the database.");

        context.Remove(entity!);
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
        CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.Id);
        CheckRetrievedProduct(entityToUpdate, "There is no entity with this ID in the database.");

        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
