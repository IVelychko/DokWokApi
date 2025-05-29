using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.ProductCategories;
using FluentValidation;

namespace Application.Validators.ProductCategories;

public sealed class UpdateProductCategoryRequestValidator : AbstractValidator<UpdateProductCategoryRequest>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public UpdateProductCategoryRequestValidator(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(CategoryExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
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
        UpdateProductCategoryRequest category, string categoryName, CancellationToken cancellationToken)
    {
        return await _productCategoryRepository.IsNameUniqueAsync(categoryName, category.Id);
    }
}
