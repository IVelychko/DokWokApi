using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Exceptions;
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
            var exception = new ValidationException("The passed model is null.");
            return new Result<ShopModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _shopRepository.AddAsync(entity);

        return result.Match(s => s.ToModel(),
            e => new Result<ShopModel>(e));
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

    public async Task<ShopModel?> GetByIdAsync(long id)
    {
        var entity = await _shopRepository.GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<ShopModel?> GetByAddressAsync(string street, string building)
    {
        var entity = await _shopRepository.GetByAddressAsync(street, building);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<bool>> IsAddressTakenAsync(string street, string building)
    {
        if (street is null)
        {
            var exception = new ValidationException("The passed street is null");
            return new Result<bool>(exception);
        }
        else if (building is null)
        {
            var exception = new ValidationException("The passed building is null");
            return new Result<bool>(exception);
        }

        var isTakenResult = await _shopRepository.IsAddressTakenAsync(street, building);
        return isTakenResult;
    }

    public async Task<Result<ShopModel>> UpdateAsync(ShopModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ShopModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _shopRepository.UpdateAsync(entity);

        return result.Match(s => s.ToModel(),
            e => new Result<ShopModel>(e));
    }
}
