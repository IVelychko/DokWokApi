using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.Orders;
using FluentValidation;

namespace Application.Validators.Orders;

public class DeleteOrderRequestValidator : AbstractValidator<DeleteOrderRequest>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderRequestValidator(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(OrderToDeleteExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no order with this ID to delete in the database");
    }

    private async Task<bool> OrderToDeleteExists(long orderId, CancellationToken cancellationToken)
    {
        return await _orderRepository.OrderExistsAsync(orderId);
    }
}
