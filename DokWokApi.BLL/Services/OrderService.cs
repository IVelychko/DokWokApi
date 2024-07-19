using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using Microsoft.EntityFrameworkCore;
using DokWokApi.BLL.Extensions;

namespace DokWokApi.BLL.Services;

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
        var queryable = _orderRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = entities.Select(o => o.ToModel());
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId)
    {
        var entities = _orderRepository.GetAllWithDetails();
        var filteredEntities = entities.Where(o => o.UserId == userId);
        var list = await filteredEntities.ToListAsync();
        var models = list.Select(o => o.ToModel());
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
