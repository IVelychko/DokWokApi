﻿using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

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
            var error = new ValidationError(validationResult.Errors);
            return Result<ProductCategory>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : Result<ProductCategory>.Failure(new DbError("There was the database error"));
        }
        else
        {
            var error = new DbError("There was the database error");
            return Result<ProductCategory>.Failure(error);
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

    public async Task<ProductCategory?> GetByIdAsync(long id)
    {
        return await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            var error = new ValidationError("The passed name is null or empty");
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
            Error error = validationResult.IsNotFound ? new EntityNotFoundError(validationResult.Errors)
                : new ValidationError(validationResult.Errors);
            return Result<ProductCategory>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : Result<ProductCategory>.Failure(new DbError("There was the database error"));
        }
        else
        {
            var error = new DbError("There was the database error");
            return Result<ProductCategory>.Failure(error);
        }
    }
}
