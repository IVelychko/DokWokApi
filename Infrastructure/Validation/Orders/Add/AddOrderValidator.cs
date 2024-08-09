using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Orders.Add;

public sealed class AddOrderValidator : AbstractValidator<AddOrderValidationModel>
{
    private readonly StoreDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    public AddOrderValidator(StoreDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;

        RuleFor(x => x).NotNull().WithMessage("The passed order is null").DependentRules(() =>
        {
            RuleFor(x => x.UserId)
                .MustAsync(async (userId, cancellationToken) =>
                {
                    return await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == userId, cancellationToken);
                })
                .WithMessage("There is no user with the ID specified in the UserId property of the Order entity")
                .When(x => x.UserId is not null);

            RuleFor(x => x.ShopId)
                .MustAsync(async (shopId, cancellationToken) =>
                {
                    return await _context.Shops.AsNoTracking().AnyAsync(s => s.Id == shopId, cancellationToken);
                })
                .WithMessage("There is no shop with the ID specified in the ShopId property of the Order entity")
                .When(x => x.ShopId is not null);
        });
    }
}
