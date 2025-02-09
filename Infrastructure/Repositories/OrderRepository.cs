using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
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

    public async Task AddAsync(Order entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(Order entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<Order>> GetAllBySpecificationAsync(Specification<Order> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.Orders, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<Order>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Order>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders.Where(o => o.UserId == userId);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Order>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<Order> query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Shop)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                    .ThenInclude(p => p!.Category);

        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Order>> GetAllWithDetailsByUserIdAsync(long userId, PageInfo? pageInfo = null)
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
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Order>(id);
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

    public void Update(Order entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
}
