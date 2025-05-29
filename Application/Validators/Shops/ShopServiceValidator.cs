using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Shops;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Validators.Shops;

public class ShopServiceValidator : IShopServiceValidator
{
    private readonly IValidator<UpdateShopRequest> _updateShopValidator;
    private readonly IValidator<AddShopRequest> _addShopValidator;
    private readonly IValidator<DeleteShopRequest> _deleteShopValidator;

    public ShopServiceValidator(
        IValidator<UpdateShopRequest> updateShopValidator,
        IValidator<AddShopRequest> addShopValidator,
        IValidator<DeleteShopRequest> deleteShopValidator)
    {
        _updateShopValidator = updateShopValidator;
        _addShopValidator = addShopValidator;
        _deleteShopValidator = deleteShopValidator;
    }
    
    public async Task<ValidationResult> ValidateUpdateShopAsync(UpdateShopRequest request)
    {
        return await _updateShopValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateAddShopAsync(AddShopRequest request)
    {
        return await _addShopValidator.ValidateAsync(request);
    }

    public async Task<ValidationResult> ValidateDeleteShopAsync(DeleteShopRequest request)
    {
        return await _deleteShopValidator.ValidateAsync(request);
    }
}