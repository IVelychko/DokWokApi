using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShopService(IShopRepository shopRepository, IUnitOfWork unitOfWork)
    {
        _shopRepository = shopRepository;
        _unitOfWork = unitOfWork;
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
        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _shopRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ShopModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var entities = await _shopRepository.GetAllAsync(pageInfo);
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
        _shopRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _shopRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");
        return updatedEntity.ToModel();
    }
}
