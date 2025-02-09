using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Orders;
using FluentValidation;

namespace Application.Operations.Order.Commands.DeleteOrder;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandValidator(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(OrderToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no order with this ID to delete in the database");
    }

    private async Task<bool> OrderToDeleteExists(long orderId, CancellationToken cancellationToken) =>
        (await _orderRepository.GetByIdAsync(orderId)) is not null;
}
