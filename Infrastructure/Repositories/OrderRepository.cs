using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared;
using Domain.Specifications.Orders;
using Infrastructure.Specifications.Orders;
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

    public async Task<IList<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }
    
    public async Task<IList<Order>> GetAllBySpecificationAsync(OrderSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = OrderSpecificationEvaluator.ApplySpecification(_context.Orders, specification);
        return await query.ToListAsync();
    }

    public async Task<IList<Order>> GetAllByUserIdAsync(long userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(long id)
    {
        return await _context.FindAsync<Order>(id);
    }
    
    public async Task<Order?> GetBySpecificationAsync(OrderSpecification specification)
    {
        Ensure.ArgumentNotNull(specification);
        var query = OrderSpecificationEvaluator.ApplySpecification(_context.Orders, specification);
        return await query.FirstOrDefaultAsync();
    }

    public void Update(Order entity)
    {
        Ensure.ArgumentNotNull(entity);
        _context.Update(entity);
    }
    
    public async Task<bool> OrderExistsAsync(long id)
    {
        var exists = await _context.Orders
            .AnyAsync(x => x.Id == id);
        return exists;
    }
}
