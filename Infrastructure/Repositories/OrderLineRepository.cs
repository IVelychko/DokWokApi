using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Models;
using Domain.ResultType;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext _context;

    public OrderLineRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderLine>> AddAsync(OrderLine entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("orderLine", "The passed entity is null");
            return Result<OrderLine>.Failure(error);
        }

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        return await GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was the database error");
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.OrderLines.FirstOrDefaultAsync(ol => ol.Id == id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<OrderLine>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines;

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        var query = _context.OrderLines.Where(ol => ol.OrderId == orderId);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
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

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        var query = _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .Where(ol => ol.OrderId == orderId);

        if (pageInfo is not null)
        {
            var itemsToSkip = (pageInfo.PageNumber - 1) * pageInfo.PageSize;
            query = query
                .Skip(itemsToSkip)
                .Take(pageInfo.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await _context.OrderLines.FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<Result<OrderLine>> UpdateAsync(OrderLine entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("orderLine", "The passed entity is null");
            return Result<OrderLine>.Failure(error);
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        return await GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was the database error");
    }
}
