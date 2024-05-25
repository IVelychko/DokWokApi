using AutoMapper;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.BLL.Models.Product;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository orderRepository;

    private readonly ICartService cartService;

    private readonly IMapper mapper;

    public OrderService(IOrderRepository orderRepository, ICartService cartService, IMapper mapper)
    {
        this.orderRepository = orderRepository;
        this.cartService = cartService;
        this.mapper = mapper;
    }

    public async Task<OrderModel> AddAsync(OrderModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");
        var entity = mapper.Map<Order>(model);

        var addedEntity = await orderRepository.AddAsync(entity);
        var addedEntityWithDetails = await orderRepository.GetByIdWithDetailsAsync(addedEntity.Id);
        return mapper.Map<OrderModel>(addedEntityWithDetails);
    }

    public async Task<OrderModel> AddOrderFromCartAsync(OrderForm form)
    {
        ServiceHelper.CheckForNull(form, "The passed model is null.");
        var model = mapper.Map<OrderModel>(form);
        var cart = await cartService.GetCart();
        ServiceHelper.ThrowIfTrue(cart.Lines.Count < 1, "There are no products in the cart");
        var orderLines = mapper.Map<List<OrderLineModel>>(cart.Lines);
        model.CreationDate = DateTime.Now;
        model.OrderLines = orderLines;
        model.TotalOrderPrice = cart.TotalCartPrice;
        
        var addedModel = await AddAsync(model);
        await cartService.ClearCart();
        return addedModel;
    }

    public async Task DeleteAsync(long id)
    {
        await orderRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<OrderModel>> GetAllAsync()
    {
        var queryable = orderRepository.GetAllWithDetails();
        var entities = await queryable.ToListAsync();
        var models = mapper.Map<IEnumerable<OrderModel>>(entities);
        return models;
    }

    public async Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId)
    {
        var entities = orderRepository.GetAllWithDetails();
        var filteredEntities = entities.Where(o => o.UserId == userId);
        var list = await filteredEntities.ToListAsync();
        var models = mapper.Map<IEnumerable<OrderModel>>(list);
        return models;
    }

    public async Task<OrderModel?> GetByIdAsync(long id)
    {
        var entity = await orderRepository.GetByIdWithDetailsAsync(id);
        if (entity is null)
        {
            return null;
        }

        var model = mapper.Map<OrderModel>(entity);
        return model;
    }

    public async Task<OrderModel> UpdateAsync(OrderModel model)
    {
        ServiceHelper.CheckForNull(model, "The passed model is null.");

        var entity = mapper.Map<Order>(model);
        var updatedEntity = await orderRepository.UpdateAsync(entity);
        var updatedEntityWithDetails = await orderRepository.GetByIdWithDetailsAsync(updatedEntity.Id);
        return mapper.Map<OrderModel>(updatedEntityWithDetails);
    }
}
