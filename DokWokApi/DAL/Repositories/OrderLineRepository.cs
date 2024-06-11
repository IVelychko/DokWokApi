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
        RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "The passed entity is null.");
        RepositoryHelper.ThrowArgumentExceptionIfTrue(entity.Quantity < 1, "The quantity must be greater than zero.");

        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
        RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(order, "There is no order with the ID specified in the OrderId property of the OrderLine entity.");
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
        product = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(product, "There is no product with the ID specified in the ProductId property of the OrderLine entity.");
        var isExisting = await _context.OrderLines.AnyAsync(ol => ol.OrderId == entity.OrderId && ol.ProductId == entity.ProductId);
        RepositoryHelper.ThrowArgumentExceptionIfTrue(isExisting, "The order line with the same orderID and productID already exists");

        if (entity.TotalLinePrice == 0)
        {
            var totalPrice = product.Price * entity.Quantity;
            entity.TotalLinePrice = totalPrice;
        }

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(OrderLine entity)
    {
        RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "The passed entity is null.");
        var entityToDelete = await _context.FindAsync<OrderLine>(entity.Id);
        RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(entityToDelete, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<OrderLine>(id);
        entity = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(entity, "There is no entity with this ID in the database.");

        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<OrderLine> GetAll()
    {
        return _context.OrderLines.AsNoTracking();
    }

    public IQueryable<OrderLine> GetAllWithDetails()
    {
        return _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category).AsNoTracking();
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await _context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        return await _context.OrderLines.AsNoTracking()
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<OrderLine> UpdateAsync(OrderLine entity)
    {
        RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "The passed entity is null.");
        RepositoryHelper.ThrowArgumentExceptionIfTrue(entity.Quantity < 1, "The quantity must be greater than zero.");

        var entityToUpdate = await _context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == entity.Id);
        entityToUpdate = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(entityToUpdate, "There is no entity with this ID in the database.");
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
        RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(order, "There is no order with the ID specified in the OrderId property of the OrderLine entity.");
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
        product = RepositoryHelper.ThrowEntityNotFoundExceptionIfNull(product, "There is no product with the ID specified in the ProductId property of the OrderLine entity.");
        if (entity.OrderId != entityToUpdate.OrderId || entity.ProductId != entityToUpdate.ProductId)
        {
            var isExisting = await _context.OrderLines.AnyAsync(ol => ol.OrderId == entity.OrderId && ol.ProductId == entity.ProductId);
            RepositoryHelper.ThrowArgumentExceptionIfTrue(isExisting, "The order line with the same orderID and productID already exists");
        }

        if (entity.TotalLinePrice == 0)
        {
            var totalPrice = product.Price * entity.Quantity;
            entity.TotalLinePrice = totalPrice;
        }

        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
