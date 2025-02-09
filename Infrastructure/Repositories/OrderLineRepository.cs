using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Models;
using Domain.Shared;
using Infrastructure.Extensions;
using Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext _context;

    public OrderLineRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OrderLine entity)
    {
        Ensure.ArgumentNotNull(entity);
        await _context.AddAsync(entity);
    }

    public void Delete(OrderLine entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Remove(entity);
    }

    public async Task<IList<OrderLine>> GetAllBySpecificationAsync(Specification<OrderLine> specification)
    {
        var query = SpecificationEvaluator.ApplySpecification(_context.OrderLines, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<OrderLine>> GetAllAsync(PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines;
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<OrderLine>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines.Where(ol => ol.OrderId == orderId);
        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<OrderLine>> GetAllWithDetailsAsync(PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category);

        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        IQueryable<OrderLine> query = _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .Where(ol => ol.OrderId == orderId);

        if (pageInfo is not null)
        {
            query = query.ApplyPagination(pageInfo);
        }

        return await query.ToListAsync();
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<OrderLine>(id);
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

    public void Update(OrderLine entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
}
