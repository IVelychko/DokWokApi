using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Specification;
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

    public async Task<IList<Product>> GetAllBySpecificationAsync(Specification<Product> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.Products, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<Product>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Product>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products.Where(p => p.CategoryId == categoryId);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Product>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products.Include(p => p.Category);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId);

        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Product>(id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNull(name);
        bool isTaken = await _context.Products.AsNoTracking().AnyAsync(c => c.Name == name);
        return !isTaken;
    }

    public void Update(Product entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
}
