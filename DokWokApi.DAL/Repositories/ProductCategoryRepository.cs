using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly StoreDbContext _context;
    private readonly IValidator<ProductCategory> _validator;

    public ProductCategoryRepository(StoreDbContext context, IValidator<ProductCategory> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<ProductCategory>> AddAsync(ProductCategory entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<ProductCategory>(exception);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var addedEntity = await GetByIdAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : new Result<ProductCategory>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<ProductCategory>(exception);
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

    public IQueryable<ProductCategory> GetAll()
    {
        return _context.ProductCategories.AsNoTracking();
    }

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Result<ProductCategory>> UpdateAsync(ProductCategory entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<ProductCategory>(exception);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var updatedEntity = await GetByIdAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : new Result<ProductCategory>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<ProductCategory>(exception);
        }
    }
}
