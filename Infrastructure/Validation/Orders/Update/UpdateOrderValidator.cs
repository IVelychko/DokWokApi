using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Orders.Update;

public sealed class UpdateOrderValidator : AbstractValidator<UpdateOrderValidationModel>
{
    private readonly StoreDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateOrderValidator(StoreDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;

        RuleFor(x => x)
            .MustAsync(OrderToUpdateExists)
            .WithName("order")
            .WithErrorCode("404")
            .WithMessage("There is no order with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.UserId)
                    .MustAsync(UserExists)
                    .WithMessage("There is no user with the ID specified in the UserId property of the Order entity")
                    .When(x => x.UserId is not null);

                RuleFor(x => x.ShopId)
                    .MustAsync(ShopExists)
                    .WithMessage("There is no shop with the ID specified in the ShopId property of the Order entity")
                    .When(x => x.ShopId is not null);
            });
    }

    private async Task<bool> OrderToUpdateExists(UpdateOrderValidationModel order, CancellationToken cancellationToken) =>
        await _context.Orders.AsNoTracking().AnyAsync(o => o.Id == order.Id, cancellationToken);

    private async Task<bool> UserExists(string? userId, CancellationToken cancellationToken) =>
        await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == userId, cancellationToken);

    private async Task<bool> ShopExists(long? shopId, CancellationToken cancellationToken) =>
        await _context.Shops.AsNoTracking().AnyAsync(s => s.Id == shopId, cancellationToken);
}
