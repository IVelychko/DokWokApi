using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;

namespace Domain.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public ShopService(IShopRepository shopRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _shopRepository = shopRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<ShopModel>> AddAsync(ShopModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("shopModel", "The passed model is null.");
            return Result<ShopModel>.Failure(error);
        }

        var entity = model.ToEntity();
        await _shopRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _shopRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveByPrefixAsync("paginatedAllShops");

        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        var entityToDelete = await _shopRepository.GetByIdAsync(id) ?? throw new DbException("There was a database error");

        await _shopRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveAsync($"shopById{id}");
        await _cacheService.RemoveAsync($"shopByAddress-{entityToDelete.Street}-{entityToDelete.Building}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllShops");
    }

    public async Task<IEnumerable<ShopModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allShops" :
            $"paginatedAllShops-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Shop> specification = new() { PageInfo = pageInfo };
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _shopRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToModel());
    }

    public async Task<ShopModel?> GetByIdAsync(long id)
    {
        string key = $"shopById{id}";
        Specification<Shop> specification = new() { Criteria = s => s.Id == id };
        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _shopRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<ShopModel?> GetByAddressAsync(string street, string building)
    {
        string key = $"shopByAddress-{street}-{building}";
        Specification<Shop> specification = new() { Criteria = s => s.Street == street && s.Building == building };
        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _shopRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<Result<bool>> IsAddressTakenAsync(string street, string building)
    {
        if (street is null || building is null)
        {
            Dictionary<string, string[]> errors = [];
            if (street is null)
            {
                errors.Add(nameof(street), ["The passed street is null"]);
            }

            if (building is null)
            {
                errors.Add(nameof(building), ["The passed building is null"]);
            }

            var error = new ValidationError(errors);
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _shopRepository.IsAddressTakenAsync(street, building);
        return isTakenResult;
    }

    public async Task<Result<ShopModel>> UpdateAsync(ShopModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("shopModel", "The passed model is null.");
            return Result<ShopModel>.Failure(error);
        }

        var entityToUpdate = await _shopRepository.GetByIdAsync(model.Id) ?? throw new DbException("There was a database error");
        await _cacheService.RemoveAsync($"shopByAddress-{entityToUpdate.Street}-{entityToUpdate.Building}");
        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveAsync($"shopById{entityToUpdate.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllShops");

        var entity = model.ToEntity();
        _shopRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _shopRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");

        return updatedEntity.ToModel();
    }
}
