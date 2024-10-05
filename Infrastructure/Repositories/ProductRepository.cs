using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
using Domain.Models;
using FluentValidation;
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

    public async Task<Result<Unit>> AddAsync(Product entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("product", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.Products.FirstAsync(p => p.Id == id);
        _context.Remove(entity);
    }

    public async Task<IEnumerable<Product>> GetAllBySpecificationAsync(Specification<Product> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.Products, specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products;
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products.Where(p => p.CategoryId == categoryId);
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products.Include(p => p.Category);
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId);

        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            var error = new ValidationError(nameof(name), "The passed name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.Products.AnyAsync(c => c.Name == name);
        return isTaken;
    }

    public Result<Unit> Update(Product entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("product", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
    }
}
