using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.Base;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;
    private readonly IValidator<Order> _validator;

    public OrderRepository(StoreDbContext context, IValidator<Order> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Order>> AddAsync(Order entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<Order>(exception);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : new Result<Order>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<Order>(exception);
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Result<Order>> UpdateAsync(Order entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<Order>(exception);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : new Result<Order>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<Order>(exception);
        }
    }
}
