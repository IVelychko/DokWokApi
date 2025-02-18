using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.OrderLines;
using FluentValidation;

namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public class AddOrderLineCommandValidator : AbstractValidator<AddOrderLineCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;

    public AddOrderLineCommandValidator(IProductRepository productRepository,
        IOrderLineRepository orderLineRepository, IOrderRepository orderRepository)
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

    private async Task<bool> OrderExists(long orderId, CancellationToken token) =>
        await _orderRepository.OrderExistsAsync(orderId);

    private async Task<bool> ProductExists(long productId, CancellationToken token) =>
        await _productRepository.ProductExistsAsync(productId);
    
    private async Task<bool> OrderLineDoesNotExist(AddOrderLineCommand orderLine, CancellationToken token)
    {
        var exists = await _orderLineRepository.OrderLineExistsAsync(orderLine.OrderId, orderLine.ProductId);
        return !exists;
    }
}
