using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.ProductCategories;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.ProductCategory.Commands.UpdateProductCategory;

public sealed class UpdateProductCategoryCommandValidator : AbstractValidator<UpdateProductCategoryCommand>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public UpdateProductCategoryCommandValidator(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(CategoryExists)
            .WithErrorCode("404")
            .WithMessage("There is no entity with this ID to update in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .Matches(RegularExpressions.RegularString)
                    .MinimumLength(3)
                    .MustAsync(IsNameUnique)
                    .WithMessage("The product category with the same Name value is already present in the database");
            });
    }
    
    private async Task<bool> CategoryExists(long categoryId, CancellationToken cancellationToken)
    {
        return await _productCategoryRepository.CategoryExistsAsync(categoryId);
    }

    private async Task<bool> IsNameUnique(
        UpdateProductCategoryCommand category, string categoryName, CancellationToken cancellationToken)
    {
        return await _productCategoryRepository.IsNameUniqueAsync(categoryName, category.Id);
    }
}
