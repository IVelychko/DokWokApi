using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public class AddDeliveryOrderLineRequestValidator : AbstractValidator<AddDeliveryOrderLineRequest>
{
    private readonly IProductRepository _productRepository;

    public AddDeliveryOrderLineRequestValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .MustAsync(ProductExists)
            .WithMessage("There is no product with the ID specified in the ProductId property of the OrderLine entity");

        RuleFor(x => x.Quantity).GreaterThan(0);
    }

    private async Task<bool> ProductExists(long productId, CancellationToken token) =>
        (await _productRepository.GetByIdAsync(productId)) is not null;
}
