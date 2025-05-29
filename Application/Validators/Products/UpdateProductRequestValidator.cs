using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Products;
using FluentValidation;

namespace Application.Validators.Products;

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public UpdateProductRequestValidator(
        IProductRepository productRepository,
        IProductCategoryRepository productCategoryRepository)
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(ProductExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
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
        return await _productRepository.ProductExistsAsync(productId);
    }
    
    private async Task<bool> CategoryExists(long categoryId, CancellationToken cancellationToken)
    {
        return await _productCategoryRepository.CategoryExistsAsync(categoryId);
    }

    private async Task<bool> IsNameUnique(
        UpdateProductRequest product, string productName, CancellationToken cancellationToken)
    {
        return await _productRepository.IsNameUniqueAsync(productName, product.Id);
    }
}
