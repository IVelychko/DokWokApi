using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Orders;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.Orders;

public class OrderServiceValidator : IOrderServiceValidator
{
    private readonly IValidator<AddDeliveryOrderRequest> _addDeliveryOrderValidator;
    private readonly IValidator<AddTakeawayOrderRequest> _addTakeawayOrderValidator;
    private readonly IValidator<DeleteOrderRequest> _deleteOrderValidator;
    private readonly IValidator<UpdateOrderRequest> _updateOrderValidator;

    public OrderServiceValidator(
        IValidator<AddDeliveryOrderRequest> addDeliveryOrderValidator,
        IValidator<AddTakeawayOrderRequest> addTakeawayOrderValidator,
        IValidator<DeleteOrderRequest> deleteOrderValidator,
        IValidator<UpdateOrderRequest> updateOrderValidator)
    {
        _addDeliveryOrderValidator = addDeliveryOrderValidator;
        _addTakeawayOrderValidator = addTakeawayOrderValidator;
        _deleteOrderValidator = deleteOrderValidator;
        _updateOrderValidator = updateOrderValidator;
    }
    
    public async Task<ValidationResult> ValidateAddDeliveryOrderAsync(AddDeliveryOrderRequest request)
    {
        return await _addDeliveryOrderValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateAddTakeawayOrderAsync(AddTakeawayOrderRequest request)
    {
        return await _addTakeawayOrderValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateDeleteOrderAsync(DeleteOrderRequest request)
    {
        return await _deleteOrderValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateUpdateOrderAsync(UpdateOrderRequest request)
    {
        return await _updateOrderValidator.ValidateAsync(request);
    }
}