using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.OrderLines;
using FluentValidation;

namespace Application.Validators.OrderLines;

public sealed class UpdateOrderLineRequestValidator : AbstractValidator<UpdateOrderLineRequest>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;

    public UpdateOrderLineRequestValidator(
        IOrderRepository orderRepository,
        IOrderLineRepository orderLineRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(OrderLineToUpdateExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
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
            .MustAsync(AreOrderAndProductIdsUnique)
            .WithName("orderLine")
            .WithMessage("The order line with the same orderID and productID already exists")
            .When(x => x.Id != 0 && x.OrderId != 0 && x.ProductId != 0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }

    private async Task<bool> OrderLineToUpdateExists(long orderLineId, CancellationToken cancellationToken)
    {
        return await _orderLineRepository.OrderLineExistsAsync(orderLineId);
    }

    private async Task<bool> OrderExists(long orderId, CancellationToken cancellationToken)
    {
        return await _orderRepository.OrderExistsAsync(orderId);
    }

    private async Task<bool> ProductExists(long productId, CancellationToken cancellationToken)
    {
        return await _productRepository.ProductExistsAsync(productId);
    }

    private async Task<bool> AreOrderAndProductIdsUnique(UpdateOrderLineRequest orderLine, CancellationToken cancellationToken)
    {
        return await _orderLineRepository
            .AreOrderAndProductIdsUniqueAsync(orderLine.OrderId, orderLine.ProductId, orderLine.Id);
    }
}