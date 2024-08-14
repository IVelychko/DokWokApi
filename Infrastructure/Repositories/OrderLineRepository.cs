using Domain.Abstractions.Repositories;
using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Errors;
using Domain.Errors.Base;
using Domain.Exceptions;
using Domain.ResultType;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly StoreDbContext _context;
    private readonly IOrderLineRepositoryValidator _validator;

    public OrderLineRepository(StoreDbContext context, IOrderLineRepositoryValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<OrderLine>> AddAsync(OrderLine entity)
    {
        var validationResult = await _validator.ValidateAddAsync(entity);
        if (!validationResult.IsValid)
        {
            var error = new ValidationError(validationResult.ToDictionary());
            return Result<OrderLine>.Failure(error);
        }

        await _context.AddAsync(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var addedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return addedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
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

    public async Task<IEnumerable<OrderLine>> GetAllByOrderIdAndPageAsync(long orderId, int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.OrderLines
            .AsNoTracking()
            .Where(ol => ol.OrderId == orderId)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllByOrderIdAsync(long orderId)
    {
        return await _context.OrderLines.AsNoTracking().Where(ol => ol.OrderId == orderId).ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.OrderLines
            .AsNoTracking()
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync()
    {
        return await _context.OrderLines
            .AsNoTracking()
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAndPageAsync(long orderId, int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.OrderLines
            .AsNoTracking()
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .Where(ol => ol.OrderId == orderId)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId)
    {
        return await _context.OrderLines
            .AsNoTracking()
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .Where(ol => ol.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderLine>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize)
    {
        var itemsToSkip = (pageNumber - 1) * pageSize;
        return await _context.OrderLines
            .AsNoTracking()
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<OrderLine?> GetByIdAsync(long id)
    {
        return await _context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByIdWithDetailsAsync(long id)
    {
        return await _context.OrderLines
            .AsNoTracking()
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(ol => ol.Id == id);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .AsNoTracking()
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId)
    {
        return await _context.OrderLines
            .AsNoTracking()
            .Include(ol => ol.Order)
            .Include(ol => ol.Product)
                .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.ProductId == productId);
    }

    public async Task<Result<OrderLine>> UpdateAsync(OrderLine entity)
    {
        var validationResult = await _validator.ValidateUpdateAsync(entity);
        if (!validationResult.IsValid)
        {
            Error error = validationResult.Errors.Exists(x => x.ErrorCode == "404") ? new EntityNotFoundError(validationResult.ToDictionary())
                : new ValidationError(validationResult.ToDictionary());
            return Result<OrderLine>.Failure(error);
        }

        _context.Update(entity);
        var result = await _context.SaveChangesAsync();
        _context.Entry(entity).State = EntityState.Detached;
        if (result > 0)
        {
            var updatedEntity = await GetByIdWithDetailsAsync(entity.Id);
            return updatedEntity ?? throw new DbException("There was the database error");
        }
        else
        {
            throw new DbException("There was the database error");
        }
    }
}
