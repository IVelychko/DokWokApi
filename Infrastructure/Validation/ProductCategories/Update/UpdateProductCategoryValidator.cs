using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.ProductCategories.Update;

public sealed class UpdateProductCategoryValidator : AbstractValidator<UpdateProductCategoryValidationModel>
{
    private readonly StoreDbContext _context;

    public UpdateProductCategoryValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .MustAsync(CategoryExists)
            .WithName("productCategory")
            .WithErrorCode("404")
            .WithMessage("There is no entity with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .MustAsync(IsNameNotTaken)
                    .WithName("name")
                    .WithMessage("The product category with the same Name value is already present in the database");
            });
    }

    private async Task<bool> CategoryExists(UpdateProductCategoryValidationModel category, CancellationToken cancellationToken)
    {
        return await _context.ProductCategories.AsNoTracking().AnyAsync(c => c.Id == category.Id, cancellationToken);
    }

    private async Task<bool> IsNameNotTaken(UpdateProductCategoryValidationModel category, CancellationToken cancellationToken)
    {
        var existingEntity = await _context.ProductCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == category.Id, cancellationToken);
        if (existingEntity is null)
        {
            return false;
        }

        if (category.Name != existingEntity.Name
            && await _context.ProductCategories.AsNoTracking().AnyAsync(c => c.Name == category.Name, cancellationToken))
        {
            return false;
        }

        return true;
    }
}
