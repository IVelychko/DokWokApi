using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class OrderLineService : IOrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;

    private readonly IMapper _mapper;

    public OrderLineService(IOrderLineRepository orderLineRepository, IMapper mapper)
    {
        _orderLineRepository = orderLineRepository;
        _mapper = mapper;
    }

    public async Task<OrderLineModel> AddAsync(OrderLineModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");
        var entity = _mapper.Map<OrderLine>(model);

        var addedEntity = await _orderLineRepository.AddAsync(entity);
        var addedEntityWithDetails = await _orderLineRepository.GetByIdWithDetailsAsync(addedEntity.Id);
        return _mapper.Map<OrderLineModel>(addedEntityWithDetails);
    }

    public async Task DeleteAsync(long id)
    {
        await _orderLineRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllAsync()
    {
        var queryable = _orderLineRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<OrderLineModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId)
    {
        var queryable = _orderLineRepository.GetAllWithDetails().Where(ol => ol.OrderId == orderId);
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<OrderLineModel>>(entities);
        return models;
    }

    public async Task<OrderLineModel?> GetByIdAsync(long id)
    {
        var entity = await _orderLineRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<OrderLineModel>(entity);
        return model;
    }

    public async Task<OrderLineModel> UpdateAsync(OrderLineModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");

        var entity = _mapper.Map<OrderLine>(model);
        var updatedEntity = await _orderLineRepository.UpdateAsync(entity);
        var updatedEntityWithDetails = await _orderLineRepository.GetByIdWithDetailsAsync(updatedEntity.Id);
        return _mapper.Map<OrderLineModel>(updatedEntityWithDetails);
    }
}
