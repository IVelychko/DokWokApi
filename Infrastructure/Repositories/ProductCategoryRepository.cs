using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.Exceptions;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext _context;
    private readonly IProductCategoryRepositoryValidator _validator;

    public ProductCategoryRepository(StoreDbContext context, IProductCategoryRepositoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<ProductCategory>> AddAsync(ProductCategory entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.ToDictionary());
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

    public async Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return await _context.ProductCategories.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<ProductCategory>> GetAllByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.ProductCategories
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
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
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
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
