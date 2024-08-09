using Domain.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.OrderLines.Update;

public sealed class UpdateOrderLineValidator : AbstractValidator<UpdateOrderLineValidationModel>
{
    private readonly StoreDbContext _context;

    public UpdateOrderLineValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed order line is null")
            .MustAsync(OrderLineToUpdateExists)
            .WithState(_ => new ValidationFailureState { IsNotFound = true })
            .WithMessage("The order line to update was not found")
            .DependentRules(() =>
            {
                RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("The quantity must be greater than zero");

                RuleFor(x => x.OrderId)
                    .MustAsync(OrderExists)
                    .WithMessage("There is no order with the ID specified in the OrderId property of the OrderLine entity");

                RuleFor(x => x.ProductId)
                    .MustAsync(ProductExists)
                    .WithMessage("There is no product with the ID specified in the ProductId property of the OrderLine entity");

                RuleFor(x => x)
                    .MustAsync(OrderLineNotExists)
                    .WithMessage("The order line with the same orderID and productID already exists");

            });
    }

    private async Task<bool> OrderLineToUpdateExists(UpdateOrderLineValidationModel orderLine, CancellationToken cancellationToken) =>
        await _context.OrderLines.AsNoTracking().AnyAsync(ol => ol.Id == orderLine.Id, cancellationToken);

    private async Task<bool> OrderExists(long orderId, CancellationToken cancellationToken) =>
        await _context.Orders.AsNoTracking().AnyAsync(x => x.Id == orderId, cancellationToken);

    private async Task<bool> ProductExists(long productId, CancellationToken cancellationToken) =>
        await _context.Products.AsNoTracking().AnyAsync(x => x.Id == productId, cancellationToken);

    private async Task<bool> OrderLineNotExists(UpdateOrderLineValidationModel orderLine, CancellationToken cancellationToken)
    {
        var existingEntity = await _context.OrderLines.AsNoTracking().FirstOrDefaultAsync(ol => ol.Id == orderLine.Id, cancellationToken);
        if (existingEntity is null)
        {
            return false;
        }

        if (orderLine.OrderId != existingEntity.OrderId || orderLine.ProductId != existingEntity.ProductId)
        {
            var exists = await _context.OrderLines.AnyAsync(ol => ol.OrderId == orderLine.OrderId && ol.ProductId == orderLine.ProductId, cancellationToken);
            if (exists)
            {
                return false;
            }
        }

        return true;
    }
}
