using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.Products;
using FluentValidation;

namespace Application.Validators.Products;

public sealed class DeleteProductRequestValidator : AbstractValidator<DeleteProductRequest>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductRequestValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ProductToDeleteExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no product with this ID to delete in the database");
    }

    private async Task<bool> ProductToDeleteExists(long productId, CancellationToken cancellationToken)
    {
        return await _productRepository.ProductExistsAsync(productId);
    }
}
