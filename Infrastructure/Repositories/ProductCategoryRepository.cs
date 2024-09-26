using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Models;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext _context;

    public ProductCategoryRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductCategory>> AddAsync(ProductCategory entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("productCategory", "The passed entity is null");
            return Result<ProductCategory>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdAsync(entity.Id);
            return addedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<ProductCategory>(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var query = _context.ProductCategories.AsNoTracking();
        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            var error = new ValidationError(nameof(name), "The passed name is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.ProductCategories.AnyAsync(c => c.Name == name);
        return isTaken;
    }

    public async Task<Result<ProductCategory>> UpdateAsync(ProductCategory entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("productCategory", "The passed entity is null");
            return Result<ProductCategory>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdAsync(entity.Id);
            return updatedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }
}
