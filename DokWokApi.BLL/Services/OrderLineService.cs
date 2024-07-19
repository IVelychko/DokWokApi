using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;
using DokWokApi.BLL.Extensions;

namespace DokWokApi.BLL.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;

    public OrderLineService(IOrderLineRepository orderLineRepository)
    {
        _orderLineRepository = orderLineRepository;
    }

    public async Task<Result<OrderLineModel>> AddAsync(OrderLineModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderLineModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _orderLineRepository.AddAsync(entity);

        return result.Match(ol => ol.ToModel(),
            e => new Result<OrderLineModel>(e));
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _orderLineRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllAsync()
    {
        var queryable = _orderLineRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = entities.Select(ol => ol.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId)
    {
        var queryable = _orderLineRepository.GetAllWithDetails().Where(ol => ol.OrderId == orderId);
        var entities = await queryable.ToListAsync();
        var models = entities.Select(ol => ol.ToModel());
        return models;
    }

    public async Task<OrderLineModel?> GetByIdAsync(long id)
    {
        var entity = await _orderLineRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<OrderLineModel?> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        var entity = await _orderLineRepository.GetByOrderAndProductIdsWithDetailsAsync(orderId, productId);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<OrderLineModel>> UpdateAsync(OrderLineModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderLineModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _orderLineRepository.UpdateAsync(entity);

        return result.Match(ol => ol.ToModel(),
            e => new Result<OrderLineModel>(e));
    }
}
