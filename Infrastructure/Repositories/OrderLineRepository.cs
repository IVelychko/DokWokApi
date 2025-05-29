using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.OrderLines;
using Infrastructure.Specifications.OrderLines;
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

    public async Task<IList<OrderLine>> GetAllAsync()
    {
        return await _context.OrderLines.ToListAsync();
    }
    
    public async Task<IList<OrderLine>> GetAllBySpecificationAsync(OrderLineSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = OrderLineSpecificationEvaluator
            .ApplySpecification(_context.OrderLines, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<OrderLine>> GetAllByOrderIdAsync(long orderId)
    {
        return await _context.OrderLines
            .Where(ol => ol.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<OrderLine>(id);
    }
    
    public async Task<OrderLine?> GetBySpecificationAsync(OrderLineSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = OrderLineSpecificationEvaluator
            .ApplySpecification(_context.OrderLines, specification);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<bool> AreOrderAndProductIdsUniqueAsync(long orderId, long productId, long orderLineIdToExclude)
    {
        var isTaken = await _context.OrderLines
            .AnyAsync(ol => ol.OrderId == orderId && ol.ProductId == productId && ol.Id != orderLineIdToExclude);
        return !isTaken;
    }

    public async Task<bool> OrderLineExistsAsync(long id)
    {
        var exists = await _context.OrderLines
            .AnyAsync(x => x.Id == id);
        return exists;
    }
    
    public async Task<bool> OrderLineExistsAsync(long orderId, long productId)
    {
        var exists = await _context.OrderLines
            .AnyAsync(x => x.OrderId == orderId && x.ProductId == productId);
        return exists;
    }

    public void Update(OrderLine entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
}
