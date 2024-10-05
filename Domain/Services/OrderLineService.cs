using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;

namespace Domain.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderLineService(IOrderLineRepository orderLineRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _orderLineRepository = orderLineRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
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

    public async Task<IEnumerable<OrderLineModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        var entities = await _orderLineRepository.GetAllWithDetailsAsync(pageInfo);
        var models = entities.Select(ol => ol.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null)
    {
        var entities = await _orderLineRepository.GetAllWithDetailsByOrderIdAsync(orderId, pageInfo);
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
