using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShopRepository : IShopRepository
{
    private readonly StoreDbContext _context;
    private readonly IValidator<Shop> _validator;

    public ShopRepository(StoreDbContext context, IValidator<Shop> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Shop>> AddAsync(Shop entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.Errors);
            return Result<Shop>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : Result<Shop>.Failure(new DbError("There was the database error"));
        }
        else
        {
            var error = new DbError("There was the database error");
            return Result<Shop>.Failure(error);
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<Shop>(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Shop>> GetAllAsync()
    {
        return await _context.Shops.AsNoTracking().ToListAsync();
    }

    public async Task<Shop?> GetByIdAsync(long id)
    {
        return await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shop?> GetByAddressAsync(string street, string building)
    {
        return await _context.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
    }

    public async Task<Result<Shop>> UpdateAsync(Shop entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.Errors);
            return Result<Shop>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : Result<Shop>.Failure(new DbError("There was the database error"));
        }
        else
        {
            var error = new DbError("There was the database error");
            return Result<Shop>.Failure(error);
        }
    }

    public async Task<Result<bool>> IsAddressTakenAsync(string street, string building)
    {
        if (string.IsNullOrEmpty(street))
        {
            var error = new ValidationError("The passed street is null or empty");
            return Result<bool>.Failure(error);
        }
        else if (string.IsNullOrEmpty(building))
        {
            var error = new ValidationError("The passed building is null or empty");
            return Result<bool>.Failure(error);
        }

        var isTaken = await _context.Shops.AnyAsync(s => s.Street == street && s.Building == building);
        return isTaken;
    }
}
