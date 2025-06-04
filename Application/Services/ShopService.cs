using Application.Extensions;
using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Abstractions.Validation;
using Domain.DTOs.Requests.Shops;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Shops;
using Domain.Shared;

namespace Application.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShopServiceValidator _validator;

    public ShopService(
        IShopRepository shopRepository,
        IUnitOfWork unitOfWork,
        IShopServiceValidator validator)
    {
        _shopRepository = shopRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ShopResponse> AddAsync(AddShopRequest request)
    {
        await ValidateAddShopRequestAsync(request);
        var entity = request.ToEntity();
        await _shopRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        await ValidateDeleteShopRequestAsync(id);
        var entityToDelete = await _shopRepository.GetByIdAsync(id);
        entityToDelete = Ensure.EntityExists(entityToDelete, "The shop was not found");
        _shopRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IList<ShopResponse>> GetAllAsync()
    {
        var entities = await _shopRepository.GetAllAsync();
        return entities.Select(p => p.ToResponse()).ToList();
    }

    public async Task<ShopResponse> GetByIdAsync(long id)
    {
        var entity = await _shopRepository.GetByIdAsync(id);
        entity = Ensure.EntityExists(entity, "The shop was not found");
        return entity.ToResponse();
    }

    public async Task<ShopResponse> GetByAddressAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        var entity = await _shopRepository.GetByAddressAsync(street, building);
        entity = Ensure.EntityExists(entity, "The shop was not found");
        return entity.ToResponse();
    }

    public async Task<IsTakenResponse> IsAddressTakenAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        var isUnique = await _shopRepository.IsAddressUniqueAsync(street, building);
        return new IsTakenResponse(!isUnique);
    }

    public async Task<ShopResponse> UpdateAsync(UpdateShopRequest request)
    {
        await ValidateUpdateShopRequestAsync(request);
        var entity = request.ToEntity();
        _shopRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToResponse();
    }
    
    private async Task ValidateDeleteShopRequestAsync(long id)
    {
        DeleteShopRequest request = new(id);
        var validationResult = await _validator.ValidateDeleteShopAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateUpdateShopRequestAsync(UpdateShopRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateUpdateShopAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
    
    private async Task ValidateAddShopRequestAsync(AddShopRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var validationResult = await _validator.ValidateAddShopAsync(request);
        validationResult.ThrowIfValidationFailed();
    }
}
