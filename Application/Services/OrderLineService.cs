using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Shared;

namespace Application.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public OrderLineService(IOrderLineRepository orderLineRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<OrderLineResponse> AddAsync(AddOrderLineCommand command)
    {
        Ensure.ArgumentNotNull(command);
        OrderLine entity = command.ToEntity();
        Product? product = await _productRepository.GetByIdAsync(entity.ProductId);
        product = Ensure.EntityFound(product, "The related product was not found");
        entity.TotalLinePrice = product.Price * entity.Quantity;
        
        await _orderLineRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        OrderLine createdEntity = await _orderLineRepository.GetByIdWithDetailsAsync(entity.Id) 
                                  ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync($"allOrderLinesByOrderId{createdEntity.OrderId}");
        await _cacheService.RemoveAsync("allOrderLines");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrderLines");

        return createdEntity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        OrderLine entityToDelete = await _orderLineRepository.GetByIdAsync(id) 
                                   ?? throw new DbException("There was a database error");

        _orderLineRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"allOrderLinesByOrderId{entityToDelete.OrderId}");
        await _cacheService.RemoveAsync($"orderLineByOrderId{entityToDelete.OrderId}-ProductId{entityToDelete.ProductId}");
        await _cacheService.RemoveAsync("allOrderLines");
        await _cacheService.RemoveAsync($"orderLineById{entityToDelete.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrderLines");
    }

    public async Task<IEnumerable<OrderLineResponse>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allOrderLines" :
            $"paginatedAllOrderLines-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<OrderLine> specification = new() { PageInfo = pageInfo };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        IList<OrderLine> entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<IEnumerable<OrderLineResponse>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? $"allOrderLinesByOrderId{orderId}" :
            $"paginatedAllOrderLinesByOrderId{orderId}-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<OrderLine> specification = new()
        {
            Criteria = ol => ol.OrderId == orderId,
            PageInfo = pageInfo
        };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        IList<OrderLine> entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<OrderLineResponse?> GetByIdAsync(long id)
    {
        string key = $"orderLineById{id}";
        Specification<OrderLine> specification = new() { Criteria = ol => ol.Id == id };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<OrderLineResponse?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        string key = $"orderLineByOrderId{orderId}-ProductId{productId}";
        Specification<OrderLine> specification = new()
        {
            Criteria = ol => ol.OrderId == orderId && ol.ProductId == productId
        };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<OrderLineResponse> UpdateAsync(UpdateOrderLineCommand command)
    {
        Ensure.ArgumentNotNull(command);
        OrderLine entity = command.ToEntity();
        Product? product = await _productRepository.GetByIdAsync(entity.ProductId);
        product = Ensure.EntityFound(product, "The related product was not found");
        entity.TotalLinePrice = product.Price * entity.Quantity;
        
        _orderLineRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        OrderLine updatedEntity = await _orderLineRepository.GetByIdWithDetailsAsync(entity.Id) 
                                  ?? throw new DbException("There was a database error");
        await _cacheService.RemoveByPrefixAsync("allOrderLinesByOrderId");
        await _cacheService.RemoveByPrefixAsync("orderLineByOrderId");
        await _cacheService.RemoveAsync("allOrderLines");
        await _cacheService.RemoveAsync($"orderLineById{entity.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllOrderLines");
        
        return updatedEntity.ToResponse();
    }
}
