using Domain.DTOs.Requests.OrderLines;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IOrderLineServiceValidator
{
    Task<ValidationResult> ValidateAddOrderLineAsync(AddOrderLineRequest request);
    
    Task<ValidationResult> ValidateUpdateOrderLineAsync(UpdateOrderLineRequest request);
    
    Task<ValidationResult> ValidateDeleteOrderLineAsync(DeleteOrderLineRequest request);
}