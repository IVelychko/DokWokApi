using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.Products;
using Infrastructure.Specifications.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;

    public ProductRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(Product entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<Product>> GetAllBySpecificationAsync(ProductSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = ProductSpecificationEvaluator.ApplySpecification(_context.Products, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<IList<Product>> GetAllByCategoryIdAsync(long categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Product>(id);
    }
    
    public async Task<Product?> GetBySpecificationAsync(ProductSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = ProductSpecificationEvaluator.ApplySpecification(_context.Products, specification);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        var isTaken = await _context.Products
            .AnyAsync(c => c.Name == name);
        return !isTaken;
    }
    
    public async Task<bool> IsNameUniqueAsync(string name, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        var isTaken = await _context.Products
            .AnyAsync(c => c.Name == name && c.Id != idToExclude);
        return !isTaken;
    }

    public void Update(Product entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
    
    public async Task<bool> ProductExistsAsync(long id)
    {
        var exists = await _context.Products
            .AnyAsync(x => x.Id == id);
        return exists;
    }
}
