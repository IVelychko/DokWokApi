using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.OrderLines;
using FluentValidation;

namespace Application.Validators.OrderLines;

public class AddOrderLineRequestValidator : AbstractValidator<AddOrderLineRequest>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;

    public AddOrderLineRequestValidator(
        IProductRepository productRepository,
        IOrderLineRepository orderLineRepository,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderLineRepository = orderLineRepository;
        _orderRepository = orderRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .MustAsync(OrderExists)
            .WithMessage("There is no order with the ID specified in the OrderId property of the OrderLine entity");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .MustAsync(ProductExists)
            .WithMessage("There is no product with the ID specified in the ProductId property of the OrderLine entity");

        RuleFor(x => x)
            .MustAsync(OrderLineDoesNotExist)
            .WithName("orderLine")
            .WithMessage("The order line with the same orderID and productID already exists")
            .When(x => x.ProductId != 0 && x.OrderId != 0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }

    private async Task<bool> OrderExists(long orderId, CancellationToken token)
    {
        return await _orderRepository.OrderExistsAsync(orderId);
    }

    private async Task<bool> ProductExists(long productId, CancellationToken token)
    {
        return await _productRepository.ProductExistsAsync(productId);
    }

    private async Task<bool> OrderLineDoesNotExist(AddOrderLineRequest orderLine, CancellationToken token)
    {
        var exists = await _orderLineRepository.OrderLineExistsAsync(orderLine.OrderId, orderLine.ProductId);
        return !exists;
    }
}
