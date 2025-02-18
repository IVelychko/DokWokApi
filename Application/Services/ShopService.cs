using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Shops;
using Domain.DTOs.Responses.Shops;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Shared;

namespace Application.Services;

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

    public async Task<ShopResponse> AddAsync(AddShopCommand command)
    {
        Ensure.ArgumentNotNull(command);
        Shop entity =  command.ToEntity();
        await _shopRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        Shop createdEntity = await _shopRepository.GetByIdAsync(entity.Id) 
                             ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveByPrefixAsync("paginatedAllShops");

        return createdEntity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        Shop entityToDelete = await _shopRepository.GetByIdAsync(id) 
                              ?? throw new DbException("There was a database error");

        _shopRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveAsync($"shopById{id}");
        await _cacheService.RemoveAsync($"shopByAddress-{entityToDelete.Street}-{entityToDelete.Building}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllShops");
    }

    public async Task<IEnumerable<ShopResponse>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allShops" :
            $"paginatedAllShops-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Shop> specification = new() { PageInfo = pageInfo };
        IList<Shop> entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _shopRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<ShopResponse?> GetByIdAsync(long id)
    {
        string key = $"shopById{id}";
        Specification<Shop> specification = new() { Criteria = s => s.Id == id };
        Shop? entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _shopRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<ShopResponse?> GetByAddressAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street);
        Ensure.ArgumentNotNullOrWhiteSpace(building);
        string key = $"shopByAddress-{street}-{building}";
        Specification<Shop> specification = new() { Criteria = s => s.Street == street && s.Building == building };
        Shop? entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _shopRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<bool> IsAddressUniqueAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street);
        Ensure.ArgumentNotNullOrWhiteSpace(building);
        return await _shopRepository.IsAddressUniqueAsync(street, building);
    }

    public async Task<ShopResponse> UpdateAsync(UpdateShopCommand command)
    {
        Ensure.ArgumentNotNull(command);
        Shop entity = command.ToEntity();
        _shopRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        Shop updatedEntity = await _shopRepository.GetByIdAsync(entity.Id) 
                             ?? throw new DbException("There was a database error");
        
        await _cacheService.RemoveByPrefixAsync("shopByAddress");
        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveAsync($"shopById{entity.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllShops");

        return updatedEntity.ToResponse();
    }
}
