using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Shops.Update;

public sealed class UpdateShopValidator : AbstractValidator<UpdateShopValidationModel>
{
    private readonly StoreDbContext _context;

    public UpdateShopValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .MustAsync(ShopToUpdateExists)
            .WithName("shop")
            .WithErrorCode("404")
            .WithMessage("There is no entity with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .MustAsync(IsAddressTaken)
                    .WithName("address")
                    .WithMessage("The shop with the same Street and Building values is already present in the database");
            });
    }

    private async Task<bool> ShopToUpdateExists(UpdateShopValidationModel shop, CancellationToken cancellationToken)
    {
        return await _context.Shops.AsNoTracking().AnyAsync(x => x.Id == shop.Id, cancellationToken);
    }

    private async Task<bool> IsAddressTaken(UpdateShopValidationModel shop, CancellationToken cancellationToken)
    {
        var existsingEntity = await _context.Shops.AsNoTracking().FirstOrDefaultAsync(x => x.Id == shop.Id, cancellationToken);
        if (existsingEntity is null)
        {
            return false;
        }

        if ((shop.Street != existsingEntity.Street || shop.Building != existsingEntity.Building) &&
            await _context.Shops.AsNoTracking().AnyAsync(s => s.Street == shop.Street && s.Building == shop.Building, cancellationToken))
        {
            return false;
        }

        return true;
    }
}
