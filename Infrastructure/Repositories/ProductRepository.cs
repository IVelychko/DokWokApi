using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Models;
using Domain.ResultType;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;

    public ProductRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Product>> AddAsync(Product entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("product", "The passed entity is null");
            return Result<Product>.Failure(error);
        }

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        return await GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was the database error");
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products;
        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        var query = _context.Products.Where(p => p.CategoryId == categoryId);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Product> query = _context.Products.Include(p => p.Category);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
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

    public async Task<Result<Product>> UpdateAsync(Product entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("product", "The passed entity is null");
            return Result<Product>.Failure(error);
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        return await GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was the database error");
    }
}
