using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Shops.Add;

public sealed class AddShopValidator : AbstractValidator<AddShopValidationModel>
{
    private readonly StoreDbContext _context;

    public AddShopValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("The passed shop is null")
            .MustAsync(IsAddressTaken)
            .WithMessage("The shop with the same Street and Building values is already present in the database");
    }

    private async Task<bool> IsAddressTaken(AddShopValidationModel shop, CancellationToken cancellationToken)
    {
        return !await _context.Shops.AsNoTracking().AnyAsync(s => s.Street == shop.Street && s.Building == shop.Building, cancellationToken);
    }
}
