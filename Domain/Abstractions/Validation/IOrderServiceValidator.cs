using Domain.DTOs.Requests.Orders;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IOrderServiceValidator
{
    Task<ValidationResult> ValidateAddDeliveryOrderAsync(AddDeliveryOrderRequest request);
    
    Task<ValidationResult> ValidateAddTakeawayOrderAsync(AddTakeawayOrderRequest request);
    
    Task<ValidationResult> ValidateDeleteOrderAsync(DeleteOrderRequest request);
    
    Task<ValidationResult> ValidateUpdateOrderAsync(UpdateOrderRequest request);
}