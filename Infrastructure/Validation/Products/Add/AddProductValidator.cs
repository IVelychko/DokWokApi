using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Validation.Products.Add;

public sealed class AddProductValidator : AbstractValidator<AddProductValidationModel>
{
    private readonly StoreDbContext _context;

    public AddProductValidator(StoreDbContext context)
    {
        _context = context;

        RuleFor(x => x).NotNull().WithMessage("The passed product is null").DependentRules(() =>
        {
            RuleFor(x => x.CategoryId)
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    return await _context.ProductCategories.AsNoTracking().AnyAsync(x => x.Id == categoryId, cancellationToken);
                })
                .WithMessage("There is no product category with the ID specified in the CategoryId property of the Product entity");

            RuleFor(x => x.Name)
                .MustAsync(async (name, cancellationToken) =>
                {
                    return !await _context.Products.AsNoTracking().AnyAsync(p => p.Name == name, cancellationToken);
                })
                .WithMessage("The product with the same Name value is already present in the database");
        });
    }
}
