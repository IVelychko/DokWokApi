using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public sealed class UpdateOrderLineCommandValidator : AbstractValidator<UpdateOrderLineCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;

    public UpdateOrderLineCommandValidator(IOrderRepository orderRepository,
        IOrderLineRepository orderLineRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(OrderLineToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no order line with this ID to update in the database");

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
            .When(x => x.Id != 0 && x.OrderId != 0 && x.ProductId != 0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }

    private async Task<bool> OrderLineToUpdateExists(long orderLineId, CancellationToken cancellationToken) =>
        (await _orderLineRepository.GetByIdAsync(orderLineId)) is not null;

    private async Task<bool> OrderExists(long orderId, CancellationToken cancellationToken) =>
        (await _orderRepository.GetByIdAsync(orderId)) is not null;

    private async Task<bool> ProductExists(long productId, CancellationToken cancellationToken) =>
        (await _productRepository.GetByIdAsync(productId)) is not null;

    private async Task<bool> OrderLineNotExists(UpdateOrderLineCommand orderLine, CancellationToken cancellationToken)
    {
        var existingEntity = await _orderLineRepository.GetByIdAsync(orderLine.Id);
        if (existingEntity is null)
        {
            return false;
        }

        if (orderLine.OrderId != existingEntity.OrderId || orderLine.ProductId != existingEntity.ProductId)
        {
            var exists = (await _orderLineRepository.GetByOrderAndProductIdsAsync(orderLine.OrderId, orderLine.ProductId)) is not null;
            if (exists)
            {
                return false;
            }
        }

        return true;
    }
}
