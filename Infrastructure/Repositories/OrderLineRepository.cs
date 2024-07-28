using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Exceptions.Base;
using Domain.ResultType;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext _context;
    private readonly IValidator<OrderLine> _validator;

    public OrderLineRepository(StoreDbContext context, IValidator<OrderLine> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<OrderLine>> AddAsync(OrderLine entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<OrderLine>(exception);
        }

        var product = await _context.Products.AsNoTracking().FirstAsync(p => p.Id == entity.ProductId);
        if (entity.TotalLinePrice == 0)
        {
            var totalPrice = product.Price * entity.Quantity;
            entity.TotalLinePrice = totalPrice;
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity is not null ? addedEntity
                : new Result<OrderLine>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<OrderLine>(exception);
        }
    }

    public async Task<bool?> DeleteByIdAsync(long id)
    {
        var entity = await _context.FindAsync<OrderLine>(id);
        if (entity is null)
        {
            return null;
        }

        _context.Remove(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<OrderLine>> GetAllAsync()
    {
        return await _context.OrderLines.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync()
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId)
    {
        return await _context.OrderLines
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category).AsNoTracking()
            .Where(ol => ol.OrderId == orderId)
            .ToListAsync();
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

    public async Task<Result<OrderLine>> UpdateAsync(OrderLine entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Exception exception = !validationResult.IsFound ? new NotFoundException(validationResult.Error)
                : new ValidationException(validationResult.Error);

            return new Result<OrderLine>(exception);
        }

        var product = await _context.Products.AsNoTracking().FirstAsync(p => p.Id == entity.ProductId);
        if (entity.TotalLinePrice == 0)
        {
            var totalPrice = product.Price * entity.Quantity;
            entity.TotalLinePrice = totalPrice;
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity is not null ? updatedEntity
                : new Result<OrderLine>(new DbException("There was the database error"));
        }
        else
        {
            var exception = new DbException("There was the database error");
            return new Result<OrderLine>(exception);
        }
    }
}
