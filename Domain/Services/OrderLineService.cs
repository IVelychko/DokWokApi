using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Domain.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _distributedCache;

    public OrderLineService(IOrderLineRepository orderLineRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IDistributedCache distributedCache)
    {
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _distributedCache = distributedCache;
    }

    public async Task<Result<OrderLineModel>> AddAsync(OrderLineModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("orderLineModel", "The passed model is null");
            return Result<OrderLineModel>.Failure(error);
        }

        if (model.TotalLinePrice == 0)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product is null)
            {
                var error = new ValidationError(nameof(product), $"Incorrect product id of the order line. The product does not exist with id: {model.ProductId}");
                return Result<OrderLineModel>.Failure(error);
            }

            model.TotalLinePrice = product.Price * model.Quantity;
        }

        var entity = model.ToEntity();
        await _orderLineRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _orderLineRepository.GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was a database error");
        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _orderLineRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    // TODO
    public async Task<IEnumerable<OrderLineModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allOrderLines" :
            $"allOrderLines-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<OrderLine> specification = new() { PageInfo = pageInfo };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        var entities = await Caching.GetCollectionFromCache(_distributedCache,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        var models = entities.Select(p => p.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? $"allOrderLinesByOrderId{orderId}" :
            $"allOrderLinesByOrderId{orderId}-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<OrderLine> specification = new()
        {
            Criteria = ol => ol.OrderId == orderId,
            PageInfo = pageInfo
        };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);
        var entities = await Caching.GetCollectionFromCache(_distributedCache,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        var models = entities.Select(p => p.ToModel());
        return models;
    }

    public async Task<OrderLineModel?> GetByIdAsync(long id)
    {
        string key = $"orderLineById{id}";
        Specification<OrderLine> specification = new() { Criteria = ol => ol.Id == id };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        var entity = await Caching.GetEntityFromCache(_distributedCache,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<OrderLineModel?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        string key = $"orderLineByOrderId{orderId}-ProductId{productId}";
        Specification<OrderLine> specification = new() { Criteria = ol => ol.OrderId == orderId && ol.ProductId == productId };
        specification.IncludeExpressions.AddRange([
            new(ol => ol.Order),
            new(ol => ol.Product!.Category)
            ]);

        var entity = await Caching.GetEntityFromCache(_distributedCache,
            key, specification, _orderLineRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<Result<OrderLineModel>> UpdateAsync(OrderLineModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("orderLineModel", "The passed model is null.");
            return Result<OrderLineModel>.Failure(error);
        }

        if (model.TotalLinePrice == 0)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product is null)
            {
                var error = new ValidationError(nameof(product), $"Incorrect product id of the order line. The product does not exist with id: {model.ProductId}");
                return Result<OrderLineModel>.Failure(error);
            }

            model.TotalLinePrice = product.Price * model.Quantity;
        }

        var entity = model.ToEntity();
        _orderLineRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _orderLineRepository.GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was a database error");
        return updatedEntity.ToModel();
    }
}
