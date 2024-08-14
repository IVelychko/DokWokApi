using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.ProductCategories.Add;

public sealed class AddProductCategoryValidator : AbstractValidator<AddProductCategoryValidationModel>
{
    private readonly StoreDbContext _context;

    public AddProductCategoryValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
                .MustAsync(async (name, cancellationToken) =>
                {
                    return !await _context.ProductCategories.AsNoTracking().AnyAsync(c => c.Name == name, cancellationToken);
                })
                .WithMessage("The product category with the same Name value is already present in the database");
    }
}
