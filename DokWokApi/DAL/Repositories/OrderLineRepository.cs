using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext _context;

    public OrderLineRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<OrderLine> AddAsync(OrderLine entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
        RepositoryHelper.CheckRetrievedEntity(order, "There is no order with the ID specified in the OrderId property of the OrderLine entity.");
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
        RepositoryHelper.CheckRetrievedEntity(product, "There is no product with the ID specified in the ProductId property of the OrderLine entity.");

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(OrderLine entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await _context.FindAsync<OrderLine>(entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToDelete, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<OrderLine>(id);
        entity = RepositoryHelper.CheckRetrievedEntity(entity, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<OrderLine> GetAll()
    {
        return _context.OrderLines;
    }

    public IQueryable<OrderLine> GetAllWithDetails()
    {
        return _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product);
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
            .FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine> UpdateAsync(OrderLine entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await _context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToUpdate, "There is no entity with this ID in the database.");
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
        RepositoryHelper.CheckRetrievedEntity(order, "There is no order with the ID specified in the OrderId property of the OrderLine entity.");
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
        RepositoryHelper.CheckRetrievedEntity(product, "There is no product with the ID specified in the ProductId property of the OrderLine entity.");

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
