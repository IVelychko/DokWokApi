using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;
    private readonly IValidator<Product> _validator;

    public ProductRepository(StoreDbContext context, IValidator<Product> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Product>> AddAsync(Product entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<Product>(exception);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : new Result<Product>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<Product>(exception);
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

    public IQueryable<Product> GetAll()
    {
        return _context.Products.AsNoTracking();
    }

    public IQueryable<Product> GetAllWithDetails()
    {
        return _context.Products.Include(p => p.Category).AsNoTracking();
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Products.Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Result<Product>> UpdateAsync(Product entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new EntityNotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<Product>(exception);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : new Result<Product>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<Product>(exception);
        }
    }
}
