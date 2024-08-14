using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.Exceptions;
using Domain.ResultType;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;
    private readonly IProductRepositoryValidator _validator;

    public ProductRepository(StoreDbContext context, IProductRepositoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Product>> AddAsync(Product entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.ToDictionary());
            return Result<Product>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<Product>(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllByCategoryIdAndPageAsync(long categoryId, int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllByCategoryIdAsync(long categoryId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Products
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAndPageAsync(long categoryId, int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Products
            .AsNoTracking()
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
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
            return Result<Product>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }
}
