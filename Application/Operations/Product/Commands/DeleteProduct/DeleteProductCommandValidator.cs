using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Products;
using FluentValidation;

namespace Application.Operations.Product.Commands.DeleteProduct;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ProductToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no product with this ID to delete in the database");
    }

    private async Task<bool> ProductToDeleteExists(long productId, CancellationToken cancellationToken) =>
        (await _productRepository.GetByIdAsync(productId)) is not null;
}
