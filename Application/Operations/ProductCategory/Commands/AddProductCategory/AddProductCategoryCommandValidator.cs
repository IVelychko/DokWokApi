using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.ProductCategories;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public sealed class AddProductCategoryCommandValidator : AbstractValidator<AddProductCategoryCommand>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public AddProductCategoryCommandValidator(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(RegularExpressions.RegularString)
            .MinimumLength(3)
            .MustAsync(IsNameUnique)
            .WithMessage("The product category with the same Name value is already present in the database");
    }
    
    private async Task<bool> IsNameUnique(string name, CancellationToken token)
    {
        return await _productCategoryRepository.IsNameUniqueAsync(name);
    }
}
