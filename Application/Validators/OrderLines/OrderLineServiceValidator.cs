using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.OrderLines;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.OrderLines;

public class OrderLineServiceValidator : IOrderLineServiceValidator
{
    private readonly IValidator<AddOrderLineRequest> _addOrderLineValidator;
    private readonly IValidator<UpdateOrderLineRequest> _updateOrderLineValidator;
    private readonly IValidator<DeleteOrderLineRequest> _deleteOrderLineValidator;

    public OrderLineServiceValidator(
        IValidator<AddOrderLineRequest> addOrderLineValidator,
        IValidator<UpdateOrderLineRequest> updateOrderLineValidator,
        IValidator<DeleteOrderLineRequest> deleteOrderLineValidator)
    {
        _addOrderLineValidator = addOrderLineValidator;
        _updateOrderLineValidator = updateOrderLineValidator;
        _deleteOrderLineValidator = deleteOrderLineValidator;
    }
    
    public async Task<ValidationResult> ValidateAddOrderLineAsync(AddOrderLineRequest request)
    {
        return await _addOrderLineValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateUpdateOrderLineAsync(UpdateOrderLineRequest request)
    {
        return await _updateOrderLineValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateDeleteOrderLineAsync(DeleteOrderLineRequest request)
    {
        return await _deleteOrderLineValidator.ValidateAsync(request);
    }
}