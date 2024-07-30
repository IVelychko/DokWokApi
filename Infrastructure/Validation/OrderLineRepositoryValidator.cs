using Domain.Abstractions.Validation;
using Domain.Entities;
using Domain.Validation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation;

public class OrderLineRepositoryValidator : IValidator<OrderLine>
{
    private readonly StoreDbContext _context;

    public OrderLineRepositoryValidator(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateAddAsync(OrderLine? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed order line is null");
            return result;
        }

        if (model.Quantity < 1)
        {
            result.IsValid = false;
            result.Errors.Add("The quantity must be greater than zero");
        }

        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == model.OrderId);
        if (order is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no order with the ID specified in the OrderId property of the OrderLine entity");
        }

        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.ProductId);
        if (product is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no product with the ID specified in the ProductId property of the OrderLine entity");
        }

        if (order is not null && product is not null)
        {
            var isExisting = await _context.OrderLines.AnyAsync(ol => ol.OrderId == model.OrderId && ol.ProductId == model.ProductId);
            if (isExisting)
            {
                result.IsValid = false;
                result.Errors.Add("The order line with the same orderID and productID already exists");
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateUpdateAsync(OrderLine? model)
    {
        ValidationResult result = new(true);
        if (model is null)
        {
            result.IsValid = false;
            result.Errors.Add("The passed order line is null");
            return result;
        }

        if (model.Quantity < 1)
        {
            result.IsValid = false;
            result.Errors.Add("The quantity must be greater than zero");
        }

        var entityToUpdate = await _context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == model.Id);
        if (entityToUpdate is null)
        {
            result.IsValid = false;
            result.IsNotFound = true;
            result.Errors.Add("There is no order line with this ID in the database");
            return result;
        }

        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == model.OrderId);
        if (order is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no order with the ID specified in the OrderId property of the OrderLine entity");
        }

        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.ProductId);
        if (product is null)
        {
            result.IsValid = false;
            result.Errors.Add("There is no product with the ID specified in the ProductId property of the OrderLine entity");
        }

        if (model.OrderId != entityToUpdate.OrderId || model.ProductId != entityToUpdate.ProductId)
        {
            var isExisting = await _context.OrderLines.AnyAsync(ol => ol.OrderId == model.OrderId && ol.ProductId == model.ProductId);
            if (isExisting)
            {
                result.IsValid = false;
                result.Errors.Add("The order line with the same orderID and productID already exists");
            }
        }

        return result;
    }
}
