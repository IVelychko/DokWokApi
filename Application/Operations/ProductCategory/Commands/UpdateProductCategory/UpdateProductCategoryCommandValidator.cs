using Domain.Abstractions.Repositories;
using Domain.Helpers;
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
            .WithMessage("There is no entity with this ID to update in the database");

        RuleFor(x => x.Name)
            .NotEmpty()
            .Matches(RegularExpressions.RegularString)
            .MinimumLength(3)
            .MustAsync(IsNameNotTaken)
            .WithMessage("The product category with the same Name value is already present in the database")
            .When(x => x.Id != 0);
    }

    private async Task<bool> CategoryExists(long categoryId, CancellationToken cancellationToken)
    {
        return (await _productCategoryRepository.GetByIdAsync(categoryId)) is not null;
    }

    private async Task<bool> IsNameNotTaken(UpdateProductCategoryCommand category, string categoryName, CancellationToken cancellationToken)
    {
        var existingEntity = await _productCategoryRepository.GetByIdAsync(category.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (categoryName != existingEntity.Name)
        {
            var result = await _productCategoryRepository.IsNameTakenAsync(categoryName);
            return result.Match(isTaken => !isTaken, error => false);
        }
        return true;
    }
}
