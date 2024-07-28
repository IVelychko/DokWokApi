using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Exceptions;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderModel>> AddAsync(OrderModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _orderRepository.AddAsync(entity);

        return result.Match(o => o.ToModel(),
            e => new Result<OrderModel>(e));
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _orderRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        var entities = await _orderRepository.GetAllWithDetailsAsync();
        var models = entities.Select(o => o.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId)
    {
        var entities = await _orderRepository.GetAllWithDetailsByUserIdAsync(userId);
        var models = entities.Select(o => o.ToModel());
        return models;
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
    {
        var entity = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = entity.ToModel();
        return model;
    }

    public async Task<Result<OrderModel>> UpdateAsync(OrderModel model)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderModel>(exception);
        }

        var entity = model.ToEntity();
        var result = await _orderRepository.UpdateAsync(entity);

        return result.Match(o => o.ToModel(),
            e => new Result<OrderModel>(e));
    }
}
