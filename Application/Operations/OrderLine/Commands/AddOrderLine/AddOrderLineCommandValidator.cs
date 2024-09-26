using Domain.Abstractions.Repositories;
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
            .MustAsync(OrderLineNotExists)
            .WithName("orderLine")
            .WithMessage("The order line with the same orderID and productID already exists")
            .When(x => x.ProductId != 0 && x.OrderId != 0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }

    private async Task<bool> OrderExists(long orderId, CancellationToken token) =>
        (await _orderRepository.GetByIdAsync(orderId)) is not null;

    private async Task<bool> ProductExists(long productId, CancellationToken token) =>
        (await _productRepository.GetByIdAsync(productId)) is not null;

    private async Task<bool> OrderLineNotExists(AddOrderLineCommand orderLine, CancellationToken token) =>
        (await _orderLineRepository.GetByOrderAndProductIdsAsync(orderLine.OrderId, orderLine.ProductId)) is null;
}
