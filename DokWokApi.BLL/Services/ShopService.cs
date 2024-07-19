using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Shop;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;
using DokWokApi.BLL.Extensions;

namespace DokWokApi.BLL.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _repository;

    public ShopService(IShopRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ShopModel>> AddAsync(ShopModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ShopModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _repository.AddAsync(entity);

        return result.Match(s => s.ToModel(),
            e => new Result<ShopModel>(e));
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ShopModel>> GetAllAsync()
    {
        var entities = await _repository.GetAll().OrderBy(s => s.Id).ToListAsync();
        var models = entities.Select(s => s.ToModel());
        return models;
    }

    public async Task<ShopModel?> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<ShopModel?> GetByAddressAsync(string street, string building)
    {
        var entity = await _repository.GetByAddressAsync(street, building);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<bool>> IsAddressTaken(string street, string building)
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
        
        var shop = await _repository.GetAll().FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
        return shop is not null;
    }

    public async Task<Result<ShopModel>> UpdateAsync(ShopModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<ShopModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _repository.UpdateAsync(entity);

        return result.Match(s => s.ToModel(),
            e => new Result<ShopModel>(e));
    }
}
