using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.OrderLines;
using FluentValidation;

namespace Application.Operations.OrderLine.Commands.DeleteOrderLine;

public sealed class DeleteOrderLineCommandValidator : AbstractValidator<DeleteOrderLineCommand>
{
    private readonly IOrderLineRepository _orderLineRepository;

    public DeleteOrderLineCommandValidator(IOrderLineRepository orderLineRepository)
    {
        _orderLineRepository = orderLineRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(OrderLineToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no order line with this ID to delete in the database");
    }

    private async Task<bool> OrderLineToDeleteExists(long orderLineId, CancellationToken cancellationToken) =>
        await _orderLineRepository.OrderLineExistsAsync(orderLineId);
}
