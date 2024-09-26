using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Models;
using Domain.ResultType;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;

    public OrderRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Order>> AddAsync(Order entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("order", "The passed entity is null");
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

    public async Task<IEnumerable<Order>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var query = _context.Orders.AsNoTracking();

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllByUserIdAsync(string userId, PageInfo? pageInfo = null)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(string userId, PageInfo? pageInfo = null)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .Where(o => o.UserId == userId);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
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
        if (entity is null)
        {
            var error = new ValidationError("order", "The passed entity is null");
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
