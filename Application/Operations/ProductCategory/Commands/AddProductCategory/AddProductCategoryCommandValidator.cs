using Domain.Abstractions.Repositories;
using Domain.Helpers;
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
            .MustAsync(async (name, _) =>
            {
                var result = await _productCategoryRepository.IsNameTakenAsync(name);
                return result.Match(isTaken => !isTaken, error => false);
            })
            .WithMessage("The product category with the same Name value is already present in the database");
    }
}
