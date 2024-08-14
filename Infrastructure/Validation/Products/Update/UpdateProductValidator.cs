using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Products.Update;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductValidationModel>
{
    private readonly StoreDbContext _context;

    public UpdateProductValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .MustAsync(ProductExists)
            .WithName("product")
            .WithErrorCode("404")
            .WithMessage("There is no entity with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.CategoryId)
                    .MustAsync(CategoryExists)
                    .WithMessage("There is no product category with the ID specified in the CategoryId property of the Product entity");

                RuleFor(x => x)
                    .MustAsync(IsNameTaken)
                    .WithName("name")
                    .WithMessage("The product with the same Name value is already present in the database");
            });
    }

    private async Task<bool> ProductExists(UpdateProductValidationModel product, CancellationToken cancellationToken)
    {
        return await _context.Products.AsNoTracking().AnyAsync(x => x.Id == product.Id, cancellationToken);
    }

    private async Task<bool> CategoryExists(long categoryId, CancellationToken cancellationToken)
    {
        return await _context.ProductCategories.AsNoTracking().AnyAsync(x => x.Id == categoryId, cancellationToken);
    }

    private async Task<bool> IsNameTaken(UpdateProductValidationModel product, CancellationToken cancellationToken)
    {
        var existingEntity = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.Id, cancellationToken);
        if (existingEntity is null)
        {
            return false;
        }

        if (product.Name != existingEntity.Name
            && await _context.Products.AsNoTracking().AnyAsync(p => p.Name == product.Name, cancellationToken))
        {
            return false;
        }

        return true;
    }
}
