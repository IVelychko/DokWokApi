using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.Operations.ProductCategory.Commands.DeleteProductCategory;

public sealed class DeleteProductCategoryCommandValidator : AbstractValidator<DeleteProductCategoryCommand>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public DeleteProductCategoryCommandValidator(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ProductCategoryToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no product category with this ID to delete in the database");
    }

    private async Task<bool> ProductCategoryToDeleteExists(long productCategoryId, CancellationToken cancellationToken) =>
        (await _productCategoryRepository.GetByIdAsync(productCategoryId)) is not null;
}
