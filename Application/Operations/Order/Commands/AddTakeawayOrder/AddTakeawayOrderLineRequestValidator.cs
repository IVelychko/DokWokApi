using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderLineRequestValidator : AbstractValidator<AddTakeawayOrderLineRequest>
{
    private readonly IProductRepository _productRepository;

    public AddTakeawayOrderLineRequestValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.ProductId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ProductExists)
            .WithMessage("There is no product with the ID specified in the ProductId property of the OrderLine entity");

        RuleFor(x => x.Quantity).GreaterThan(0);
    }

    private async Task<bool> ProductExists(long productId, CancellationToken token) =>
        await _productRepository.ProductExistsAsync(productId);
}
