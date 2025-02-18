using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext _context;

    public ProductCategoryRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProductCategory entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(ProductCategory entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<ProductCategory>> GetAllBySpecificationAsync(Specification<ProductCategory> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.ProductCategories, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<ProductCategory>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<ProductCategory> query = _context.ProductCategories;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<ProductCategory>(id);
    }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name);
        bool isTaken = await _context.ProductCategories
            .AnyAsync(c => c.Name == name);
        return !isTaken;
    }
    
    public async Task<bool> IsNameUniqueAsync(string name, long idToExclude)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name);
        bool isTaken = await _context.ProductCategories
            .AnyAsync(c => c.Name == name && c.Id != idToExclude);
        return !isTaken;
    }

    public void Update(ProductCategory entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
    
    public async Task<bool> CategoryExistsAsync(long id)
    {
        var exists = await _context.ProductCategories
            .AnyAsync(x => x.Id == id);
        return exists;
    }
}
