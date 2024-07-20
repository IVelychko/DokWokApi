using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

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
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<Shop>(exception);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : new Result<Shop>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<Shop>(exception);
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

    public IQueryable<Shop> GetAll()
    {
        return _context.Shops.AsNoTracking();
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
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<Shop>(exception);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : new Result<Shop>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<Shop>(exception);
        }
    }
}
