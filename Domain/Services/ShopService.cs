using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;

    public ShopService(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }

    public async Task<Result<ShopModel>> AddAsync(ShopModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("shopModel", "The passed model is null.");
            return Result<ShopModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _shopRepository.AddAsync(entity);
        return result.Match(s => s.ToModel(), Result<ShopModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _shopRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ShopModel>> GetAllAsync()
    {
        var entities = await _shopRepository.GetAllAsync();
        var models = entities.Select(s => s.ToModel());
        return models;
    }

    public async Task<IEnumerable<ShopModel>> GetAllByPageAsync(int pageNumber, int pageSize)
    {
        var entities = await _shopRepository.GetAllByPageAsync(pageNumber, pageSize);
        var models = entities.Select(s => s.ToModel());
        return models;
    }

    public async Task<ShopModel?> GetByIdAsync(long id)
    {
        var entity = await _shopRepository.GetByIdAsync(id);
        return entity?.ToModel();
    }

    public async Task<ShopModel?> GetByAddressAsync(string street, string building)
    {
        var entity = await _shopRepository.GetByAddressAsync(street, building);
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

        var entity = model.ToEntity();
        var result = await _shopRepository.UpdateAsync(entity);
        return result.Match(s => s.ToModel(), Result<ShopModel>.Failure);
    }
}
