using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.OrderLines.Add;

public sealed class AddOrderLineValidator : AbstractValidator<AddOrderLineValidationModel>
{
    private readonly StoreDbContext _context;

    public AddOrderLineValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x).NotNull().WithMessage("The passed order line is null").DependentRules(() =>
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("The quantity must be greater than zero");

            RuleFor(x => x.OrderId)
                .MustAsync(async (orderId, cancellationToken) =>
                {
                    return await _context.Orders.AsNoTracking().AnyAsync(x => x.Id == orderId, cancellationToken);
                })
                .WithMessage("There is no order with the ID specified in the OrderId property of the OrderLine entity");

            RuleFor(x => x.ProductId)
                .MustAsync(async (productId, cancellationToken) =>
                {
                    return await _context.Products.AsNoTracking().AnyAsync(x => x.Id == productId, cancellationToken);
                })
                .WithMessage("There is no product with the ID specified in the ProductId property of the OrderLine entity");

            RuleFor(x => x)
                .MustAsync(async (orderLine, cancellationToken) =>
                {
                    return !await _context.OrderLines.AnyAsync(ol => ol.OrderId == orderLine.OrderId && ol.ProductId == orderLine.ProductId, cancellationToken);
                })
                .WithMessage("The order line with the same orderID and productID already exists");
        });
    }
}
