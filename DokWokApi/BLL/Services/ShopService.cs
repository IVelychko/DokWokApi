using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.ProductCategory;
using DokWokApi.BLL.Models.Shop;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class ShopService : IShopService
{
    private readonly IShopRepository _repository;

    private readonly IMapper _mapper;

    public ShopService(IShopRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ShopModel> AddAsync(ShopModel model)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");
        var entity = _mapper.Map<Shop>(model);
        var addedEntity = await _repository.AddAsync(entity);
        return _mapper.Map<ShopModel>(addedEntity);
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ShopModel>> GetAllAsync()
    {
        var entities = await _repository.GetAll().OrderBy(s => s.Id).ToListAsync();
        var models = _mapper.Map<IEnumerable<ShopModel>>(entities);
        return models;
    }

    public async Task<ShopModel?> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<ShopModel>(entity);
        return model;
    }

    public async Task<ShopModel?> GetByAddressAsync(string street, string building)
    {
        var entity = await _repository.GetByAddressAsync(street, building);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<ShopModel>(entity);
        return model;
    }

    public async Task<bool> IsAddressTaken(string street, string building)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(street, "Street is null");
        ServiceHelper.ThrowArgumentNullExceptionIfNull(building, "Building is null");
        var shop = await _repository.GetAll().FirstOrDefaultAsync(s => s.Street == street && s.Building == building);
        return shop is not null;
    }

    public async Task<ShopModel> UpdateAsync(ShopModel model)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");

        var entity = _mapper.Map<Shop>(model);
        var updatedEntity = await _repository.UpdateAsync(entity);
        return _mapper.Map<ShopModel>(updatedEntity);
    }
}
