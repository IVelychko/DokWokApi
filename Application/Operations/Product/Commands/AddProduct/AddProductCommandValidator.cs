using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Product.Commands.AddProduct;

public sealed class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IProductRepository _productRepository;

    public AddProductCommandValidator(IProductCategoryRepository productCategoryRepository, IProductRepository productRepository)
    {
        _productCategoryRepository = productCategoryRepository;
        _productRepository = productRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty()
            .Matches(RegularExpressions.RegularString)
            .MinimumLength(3)
            .MustAsync(IsNameUnique)
            .WithMessage("The product with the same Name value is already present in the database");

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Weight)
            .GreaterThan(0);

        RuleFor(x => x.MeasurementUnit)
            .NotEmpty()
            .Matches(RegularExpressions.RegularString);

        RuleFor(x => x.Description)
            .NotEmpty()
            .Matches(RegularExpressions.RegularString)
            .MinimumLength(5);

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .MustAsync(CategoryExists)
            .WithMessage("There is no product category with the ID specified in the CategoryId property of the Product entity");
    }

    private async Task<bool> IsNameUnique(string name, CancellationToken token)
    {
        var result = await _productRepository.IsNameTakenAsync(name);
        return result.Match(isTaken => !isTaken, error => false);
    }

    private async Task<bool> CategoryExists(long categoryId, CancellationToken token) =>
        (await _productCategoryRepository.GetByIdAsync(categoryId)) is not null;
}
