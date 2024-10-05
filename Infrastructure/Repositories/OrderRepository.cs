using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Helpers;
using Domain.Models;
using FluentValidation;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;

    public OrderRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> AddAsync(Order entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("order", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        await _context.AddAsync(entity);
        return Unit.Default;
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.Orders.FirstAsync(o => o.Id == id);
        _context.Remove(entity);
    }

    public async Task<IEnumerable<Order>> GetAllBySpecificationAsync(Specification<Order> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.Orders, specification);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders;
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders.Where(o => o.UserId == userId);
        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category);

        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(long userId, PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .Where(o => o.UserId == userId);

        if (pageInfo is not null)
        {
            query = SpecificationEvaluator.ApplyPagination(query, pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public Result<Unit> Update(Order entity)
    {
        if (entity is null)
        {
            var error = new ValidationError("order", "The passed entity is null");
            return Result<Unit>.Failure(error);
        }

        _context.Update(entity);
        return Unit.Default;
    }
}
