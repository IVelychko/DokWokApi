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

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;
    private readonly IOrderRepositoryValidator _validator;

    public OrderRepository(StoreDbContext context, IOrderRepositoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Order>> AddAsync(Order entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.ToDictionary());
            return Result<Order>.Failure(error);
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

    public async Task<IEnumerable<Order>> GetAllByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Orders
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllByUserIdAndPageAsync(string userId, int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAndPageAsync(string userId, int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .Where(o => o.UserId == userId)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(string userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
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
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Result<Order>> UpdateAsync(Order entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
            return Result<Order>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
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
