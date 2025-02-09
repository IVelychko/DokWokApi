using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Products;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.Product.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public UpdateProductCommandValidator(IProductRepository productRepository, IProductCategoryRepository productCategoryRepository)
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(ProductExists)
            .WithErrorCode("404")
            .WithMessage("There is no entity with this ID in the database")
            .DependentRules(() =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .Matches(RegularExpressions.RegularString)
                    .MinimumLength(3)
                    .MustAsync(IsNameUnique)
                    .WithMessage("The product with the same Name value is already present in the database");
            });

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

    private async Task<bool> ProductExists(long productId, CancellationToken cancellationToken)
    {
        return (await _productRepository.GetByIdAsync(productId)) is not null;
    }

    private async Task<bool> CategoryExists(long categoryId, CancellationToken cancellationToken)
    {
        return (await _productCategoryRepository.GetByIdAsync(categoryId)) is not null;
    }

    private async Task<bool> IsNameUnique(UpdateProductCommand product, string productName, CancellationToken cancellationToken)
    {
        var existingEntity = await _productRepository.GetByIdAsync(product.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (productName != existingEntity.Name)
        {
            var result = await _productRepository.IsNameTakenAsync(productName);
            return result.Match(isTaken => !isTaken, error => false);
        }

        return true;
    }
}
