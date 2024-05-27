﻿using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;

    public ProductRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Product> AddAsync(Product entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var category = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.CategoryId);
        RepositoryHelper.CheckRetrievedEntity(category, "There is no product category with the ID specified in the CategoryId property of the Product entity.");
        RepositoryHelper.ThrowIfExists(await _context.Products.AnyAsync(p => p.Name == entity.Name),
            "The entity with the same Name value is already present in the database.");
        
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Product entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await _context.FindAsync<Product>(entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToDelete, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<Product>(id);
        entity = RepositoryHelper.CheckRetrievedEntity(entity, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<Product> GetAll()
    {
        return _context.Products;
    }

    public IQueryable<Product> GetAllWithDetails()
    {
        return _context.Products.Include(p => p.Category);
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Product>(id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.Id);
        entityToUpdate = RepositoryHelper.CheckRetrievedEntity(entityToUpdate, "There is no entity with this ID in the database.");
        var category = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == entity.CategoryId);
        RepositoryHelper.CheckRetrievedEntity(category, "There is no product category with the ID specified in the CategoryId property of the Product entity.");
        if (entity.Name != entityToUpdate.Name)
        {
            RepositoryHelper.ThrowIfExists(await _context.Products.AnyAsync(p => p.Name == entity.Name),
            "The entity with the same Name value is already present in the database.");
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
