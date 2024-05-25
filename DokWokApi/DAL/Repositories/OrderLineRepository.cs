using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext context;

    public OrderLineRepository(StoreDbContext context)
    {
        this.context = context;
    }

    public async Task<OrderLine> AddAsync(OrderLine entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var order = await context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
        RepositoryHelper.CheckRetrievedEntity(order, "There is no order with the ID specified in the OrderId property of the OrderLine entity.");
        var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
        RepositoryHelper.CheckRetrievedEntity(product, "There is no product with the ID specified in the ProductId property of the OrderLine entity.");

        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(OrderLine entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToDelete = await context.FindAsync<OrderLine>(entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToDelete, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await context.FindAsync<OrderLine>(id);
        entity = RepositoryHelper.CheckRetrievedEntity(entity, "There is no entity with this ID in the database.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public IQueryable<OrderLine> GetAll()
    {
        return context.OrderLines;
    }

    public IQueryable<OrderLine> GetAllWithDetails()
    {
        return context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product);
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await context.FindAsync<OrderLine>(id);
    }

    public async Task<OrderLine?> GetByIdWithDetailsAsync(long id)
    {
        return await context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
            .FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine> UpdateAsync(OrderLine entity)
    {
        RepositoryHelper.CheckForNull(entity, "The passed entity is null.");
        var entityToUpdate = await context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == entity.Id);
        RepositoryHelper.CheckRetrievedEntity(entityToUpdate, "There is no entity with this ID in the database.");
        var order = await context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
        RepositoryHelper.CheckRetrievedEntity(order, "There is no order with the ID specified in the OrderId property of the OrderLine entity.");
        var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
        RepositoryHelper.CheckRetrievedEntity(product, "There is no product with the ID specified in the ProductId property of the OrderLine entity.");

        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
