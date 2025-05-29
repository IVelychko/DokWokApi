using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.ProductCategories;
using FluentValidation;

namespace Application.Validators.ProductCategories;

public sealed class DeleteProductCategoryRequestValidator : AbstractValidator<DeleteProductCategoryRequest>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public DeleteProductCategoryRequestValidator(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ProductCategoryToDeleteExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no product category with this ID to delete in the database");
    }
    
    private async Task<bool> ProductCategoryToDeleteExists(long productCategoryId, CancellationToken cancellationToken)
    {
        return await _productCategoryRepository.CategoryExistsAsync(productCategoryId);
    }
}
