using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;

    public OrderLineService(IOrderLineRepository orderLineRepository, IProductRepository productRepository)
    {
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<OrderLineModel>> AddAsync(OrderLineModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("The passed model is null");
            return Result<OrderLineModel>.Failure(error);
        }

        if (model.TotalLinePrice == 0)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product is null)
            {
                var error = new ValidationError($"Incorrect product id of the order line. The product does not exist with id: {model.ProductId}");
                return Result<OrderLineModel>.Failure(error);
            }

            model.TotalLinePrice = product.Price * model.Quantity;
        }

        var entity = model.ToEntity();
        var result = await _orderLineRepository.AddAsync(entity);
        return result.Match(ol => ol.ToModel(), Result<OrderLineModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _orderLineRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllAsync()
    {
        var entities = await _orderLineRepository.GetAllWithDetailsAsync();
        var models = entities.Select(ol => ol.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId)
    {
        var entities = await _orderLineRepository.GetAllWithDetailsByOrderIdAsync(orderId);
        var models = entities.Select(ol => ol.ToModel());
        return models;
    }

    public async Task<OrderLineModel?> GetByIdAsync(long id)
    {
        var entity = await _orderLineRepository.GetByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<OrderLineModel?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        var entity = await _orderLineRepository.GetByOrderAndProductIdsWithDetailsAsync(orderId, productId);
        return entity?.ToModel();
    }

    public async Task<Result<OrderLineModel>> UpdateAsync(OrderLineModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("The passed model is null.");
            return Result<OrderLineModel>.Failure(error);
        }

        if (model.TotalLinePrice == 0)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product is null)
            {
                var error = new ValidationError($"Incorrect product id of the order line. The product does not exist with id: {model.ProductId}");
                return Result<OrderLineModel>.Failure(error);
            }

            model.TotalLinePrice = product.Price * model.Quantity;
        }

        var entity = model.ToEntity();
        var result = await _orderLineRepository.UpdateAsync(entity);
        return result.Match(ol => ol.ToModel(), Result<OrderLineModel>.Failure);
    }
}
