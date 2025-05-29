using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.OrderLines;
using FluentValidation;

namespace Application.Validators.OrderLines;

public sealed class DeleteOrderLineRequestValidator : AbstractValidator<DeleteOrderLineRequest>
{
    private readonly IOrderLineRepository _orderLineRepository;

    public DeleteOrderLineRequestValidator(IOrderLineRepository orderLineRepository)
    {
        _orderLineRepository = orderLineRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(OrderLineToDeleteExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no order line with this ID to delete in the database");
    }

    private async Task<bool> OrderLineToDeleteExists(long orderLineId, CancellationToken cancellationToken)
    {
        return await _orderLineRepository.OrderLineExistsAsync(orderLineId);
    }
}
