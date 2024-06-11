using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    private readonly ICartService _cartService;

    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, ICartService cartService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cartService = cartService;
        _mapper = mapper;
    }

    public async Task<OrderModel> AddAsync(OrderModel model)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");
        var entity = _mapper.Map<Order>(model);

        var addedEntity = await _orderRepository.AddAsync(entity);
        var addedEntityWithDetails = await _orderRepository.GetByIdWithDetailsAsync(addedEntity.Id);
        return _mapper.Map<OrderModel>(addedEntityWithDetails);
    }

    public async Task<OrderModel> AddOrderFromCartAsync(OrderForm form)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(form, "The passed model is null.");
        var model = _mapper.Map<OrderModel>(form);
        var cart = await _cartService.GetCart();
        ServiceHelper.ThrowOrderExceptionIfTrue(cart.Lines.Count < 1, "There are no products in the cart");
        var orderLines = _mapper.Map<List<OrderLineModel>>(cart.Lines);
        model.CreationDate = DateTime.Now;
        model.OrderLines = orderLines;
        model.TotalOrderPrice = cart.TotalCartPrice;
        model.Status = OrderStatuses.BeingProcessed;

        var addedModel = await AddAsync(model);
        await _cartService.ClearCart();
        return addedModel;
    }

    public async Task DeleteAsync(long id)
    {
        await _orderRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        var queryable = _orderRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = _mapper.Map<IEnumerable<OrderModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId)
    {
        var entities = _orderRepository.GetAllWithDetails();
        var filteredEntities = entities.Where(o => o.UserId == userId);
        var list = await filteredEntities.ToListAsync();
        var models = _mapper.Map<IEnumerable<OrderModel>>(list);
        return models;
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
    {
        var entity = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = _mapper.Map<OrderModel>(entity);
        return model;
    }

    public async Task<OrderModel> UpdateAsync(OrderModel model)
    {
        ServiceHelper.ThrowArgumentNullExceptionIfNull(model, "The passed model is null.");

        var entity = _mapper.Map<Order>(model);
        var updatedEntity = await _orderRepository.UpdateAsync(entity);
        var updatedEntityWithDetails = await _orderRepository.GetByIdWithDetailsAsync(updatedEntity.Id);
        return _mapper.Map<OrderModel>(updatedEntityWithDetails);
    }

    public async Task CompleteOrder(long id)
    {
        var entity = await _orderRepository.GetByIdAsync(id);
        entity = RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "There is no entity with the passed ID in the database.");
        entity.Status = OrderStatuses.Completed;
        await _orderRepository.UpdateAsync(entity);
    }

    public async Task CancelOrder(long id)
    {
        var entity = await _orderRepository.GetByIdAsync(id);
        entity = RepositoryHelper.ThrowArgumentNullExceptionIfNull(entity, "There is no entity with the passed ID in the database.");
        entity.Status = OrderStatuses.Cancelled;
        await _orderRepository.UpdateAsync(entity);
    }
}
