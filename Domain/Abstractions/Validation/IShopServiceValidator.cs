using Domain.DTOs.Requests.Shops;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IShopServiceValidator
{
    Task<ValidationResult> ValidateUpdateShopAsync(UpdateShopRequest request);
    
    Task<ValidationResult> ValidateAddShopAsync(AddShopRequest request);
    
    Task<ValidationResult> ValidateDeleteShopAsync(DeleteShopRequest request);
}